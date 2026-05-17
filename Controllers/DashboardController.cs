using Invexa.Data;
using Invexa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invexa.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var invoices = await _context.Invoices.AsNoTracking().ToListAsync();

            var model = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalSuppliers = await _context.Suppliers.CountAsync(),
                LowStockProducts = await _context.Products.Where(product => product.Quantity < 10).OrderBy(product => product.Quantity).Take(5).ToListAsync(),
                RecentProducts = await _context.Products.OrderByDescending(product => product.CreatedAt).Take(5).ToListAsync(),
                RecentInvoices = invoices.OrderByDescending(invoice => invoice.InvoiceDate).Take(5).ToList(),
                TotalRevenue = invoices.Sum(invoice => invoice.TotalAmount),
                MonthlyRevenue = invoices.Where(invoice => invoice.InvoiceDate >= monthStart).Sum(invoice => invoice.TotalAmount),
                TodayRevenue = invoices.Where(invoice => invoice.InvoiceDate >= today).Sum(invoice => invoice.TotalAmount)
            };

            return View(model);
        }
    }
}
