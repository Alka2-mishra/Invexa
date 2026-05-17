using Invexa.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Invexa.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(entity =>
            {
                entity.Property(product => product.Price).HasPrecision(18, 2);
                entity.Property(product => product.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            builder.Entity<Invoice>(entity =>
            {
                entity.Property(invoice => invoice.TotalAmount).HasPrecision(18, 2);
            });

            builder.Entity<InvoiceItem>(entity =>
            {
                entity.Property(item => item.Price).HasPrecision(18, 2);
            });
        }
    }
}
