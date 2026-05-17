using Invexa.Models;

namespace Invexa.ViewModels
{
    public class ReportViewModel
    {
        public decimal TodayRevenue { get; set; }

        public decimal MonthlyRevenue { get; set; }

        public int LowStockCount { get; set; }

        public int TotalInvoices { get; set; }

        public List<Product> LowStockProducts { get; set; } = new();

        public List<Invoice> RecentInvoices { get; set; } = new();
    }
}