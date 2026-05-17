using Invexa.Models;

namespace Invexa.ViewModels
{
    public class InvoiceViewModel
    {
        public string? CustomerName { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.Today;

        public List<InvoiceLineViewModel> Items { get; set; } = new();

        public Invoice? Invoice { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; } = new();
    }

    public class InvoiceLineViewModel
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}