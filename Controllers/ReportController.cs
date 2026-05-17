using Invexa.Data;
using Invexa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invexa.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var invoices = await _context.Invoices.AsNoTracking().ToListAsync();

            var model = new ReportViewModel
            {
                TodayRevenue = invoices.Where(invoice => invoice.InvoiceDate >= today).Sum(invoice => invoice.TotalAmount),
                MonthlyRevenue = invoices.Where(invoice => invoice.InvoiceDate >= monthStart).Sum(invoice => invoice.TotalAmount),
                LowStockProducts = await _context.Products.Where(product => product.Quantity < 10).OrderBy(product => product.Quantity).ToListAsync(),
                RecentInvoices = invoices.OrderByDescending(invoice => invoice.InvoiceDate).Take(10).ToList(),
                TotalInvoices = invoices.Count
            };

            model.LowStockCount = model.LowStockProducts.Count;

            return View(model);
        }
    }
}