using Invexa.Data;
using Invexa.Models;
using Invexa.Services;
using Invexa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Invexa.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly InvoiceService _invoiceService;

        public InvoiceController(ApplicationDbContext context, InvoiceService invoiceService)
        {
            _context = context;
            _invoiceService = invoiceService;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(invoice => invoice.Items)
                    .ThenInclude(item => item.Product)
                .OrderByDescending(invoice => invoice.InvoiceDate)
                .ToListAsync();

            return View(invoices);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateProductsAsync();

            var model = new InvoiceViewModel
            {
                InvoiceDate = DateTime.Today,
                Items = new List<InvoiceLineViewModel>
                {
                    new(),
                    new()
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceViewModel model)
        {
            if (model.Items.Count == 0 || model.Items.All(item => item.ProductId == 0 || item.Quantity <= 0))
            {
                ModelState.AddModelError(string.Empty, "Add at least one product line.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateProductsAsync();
                return View(model);
            }

            var productIds = model.Items.Where(item => item.ProductId > 0 && item.Quantity > 0).Select(item => item.ProductId).Distinct().ToList();
            var products = await _context.Products.Where(product => productIds.Contains(product.Id)).ToDictionaryAsync(product => product.Id);

            var invoice = new Invoice
            {
                CustomerName = model.CustomerName,
                InvoiceDate = model.InvoiceDate
            };

            foreach (var line in model.Items.Where(item => item.ProductId > 0 && item.Quantity > 0))
            {
                if (!products.TryGetValue(line.ProductId, out var product))
                {
                    continue;
                }

                invoice.Items.Add(new InvoiceItem
                {
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    Price = product.Price
                });
            }

            if (invoice.Items.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No valid invoice items were selected.");
                await PopulateProductsAsync();
                return View(model);
            }

            invoice.TotalAmount = _invoiceService.CalculateTotal(invoice.Items);

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Print), new { id = invoice.Id });
        }

        public async Task<IActionResult> Print(int id)
        {
            var invoice = await _context.Invoices
                .Include(item => item.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        private async Task PopulateProductsAsync()
        {
            ViewBag.Products = new SelectList(await _context.Products.OrderBy(product => product.Name).ToListAsync(), "Id", "Name");
        }
    }
}