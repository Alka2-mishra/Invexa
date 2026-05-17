using Invexa.Models;

namespace Invexa.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }

        public int TotalCategories { get; set; }

        public int TotalSuppliers { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal MonthlyRevenue { get; set; }

        public decimal TodayRevenue { get; set; }

        public List<Product> LowStockProducts { get; set; } = new();

        public List<Product> RecentProducts { get; set; } = new();

        public List<Invoice> RecentInvoices { get; set; } = new();
    }
}