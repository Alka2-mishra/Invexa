using Invexa.Models;

namespace Invexa.Services
{
    public class InvoiceService
    {
        public decimal CalculateTotal(IEnumerable<InvoiceItem> items)
        {
            return items.Sum(item => item.Price * item.Quantity);
        }
    }
}