using System.ComponentModel.DataAnnotations;

namespace Invexa.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public string? CustomerName { get; set; }

        public decimal TotalAmount { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();
    }
}
