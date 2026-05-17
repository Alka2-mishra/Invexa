using Invexa.Models;
using Microsoft.EntityFrameworkCore;

namespace Invexa.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite" && context.Database.CanConnect())
            {
                var missingProductColumn = !ColumnExists(context, "Products", "CreatedAt");
                var missingInvoiceColumn = !ColumnExists(context, "Invoices", "CustomerName");

                if (missingProductColumn || missingInvoiceColumn)
                {
                    context.Database.EnsureDeleted();
                }
            }

            context.Database.EnsureCreated();

            SeedCategories(context);
            SeedSuppliers(context);
            SeedProducts(context);
            SeedInvoices(context);
        }

        private static void SeedCategories(ApplicationDbContext context)
        {
            var seedCategories = new[]
            {
                "Electronics",
                "Office Supplies",
                "Networking",
                "Furniture",
                "Stationery",
                "Cleaning"
            };

            var existing = context.Categories.Select(category => category.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var newCategories = seedCategories
                .Where(name => !existing.Contains(name))
                .Select(name => new Category { Name = name })
                .ToList();

            if (newCategories.Count > 0)
            {
                context.Categories.AddRange(newCategories);
                context.SaveChanges();
            }
        }

        private static void SeedSuppliers(ApplicationDbContext context)
        {
            var seedSuppliers = new[]
            {
                new Supplier { Name = "Nova Supply Co.", Email = "sales@novasupply.com", Phone = "(+1) 202-555-0101" },
                new Supplier { Name = "Vertex Trading", Email = "contact@vertextrading.com", Phone = "(+1) 202-555-0112" },
                new Supplier { Name = "Omega Office Mart", Email = "support@omegaoffice.com", Phone = "(+1) 202-555-0123" },
                new Supplier { Name = "Prime Hardware Hub", Email = "hello@primehardwarehub.com", Phone = "(+1) 202-555-0134" }
            };

            var existing = context.Suppliers.Select(supplier => supplier.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var newSuppliers = seedSuppliers.Where(supplier => !existing.Contains(supplier.Name)).ToList();

            if (newSuppliers.Count > 0)
            {
                context.Suppliers.AddRange(newSuppliers);
                context.SaveChanges();
            }
        }

        private static void SeedProducts(ApplicationDbContext context)
        {
            var categories = context.Categories.ToDictionary(category => category.Name, category => category.Id, StringComparer.OrdinalIgnoreCase);
            var suppliers = context.Suppliers.ToDictionary(supplier => supplier.Name, supplier => supplier.Id, StringComparer.OrdinalIgnoreCase);

            var seedProducts = new[]
            {
                new SeedProduct("Wireless Mouse", "Ergonomic 2.4GHz wireless mouse", 24.99m, 42, "Electronics", "Omega Office Mart", DateTime.UtcNow.AddDays(-30)),
                new SeedProduct("Mechanical Keyboard", "RGB tenkeyless keyboard", 79.00m, 18, "Electronics", "Nova Supply Co.", DateTime.UtcNow.AddDays(-22)),
                new SeedProduct("HDMI Cable 2m", "High-speed HDMI 2.1 certified cable", 12.50m, 95, "Networking", "Prime Hardware Hub", DateTime.UtcNow.AddDays(-19)),
                new SeedProduct("Wi-Fi Router AX3000", "Dual-band router for office network", 139.00m, 7, "Networking", "Prime Hardware Hub", DateTime.UtcNow.AddDays(-17)),
                new SeedProduct("A4 Copy Paper Pack", "500 sheets premium white paper", 8.75m, 130, "Office Supplies", "Vertex Trading", DateTime.UtcNow.AddDays(-14)),
                new SeedProduct("Stapler", "Heavy-duty metal stapler", 16.20m, 24, "Office Supplies", "Vertex Trading", DateTime.UtcNow.AddDays(-12)),
                new SeedProduct("Office Chair", "Adjustable lumbar support chair", 189.99m, 6, "Furniture", "Nova Supply Co.", DateTime.UtcNow.AddDays(-10)),
                new SeedProduct("Desk Lamp", "LED desk lamp with dimmer", 29.40m, 15, "Furniture", "Omega Office Mart", DateTime.UtcNow.AddDays(-8)),
                new SeedProduct("Whiteboard Markers", "4-color marker set", 6.80m, 52, "Stationery", "Vertex Trading", DateTime.UtcNow.AddDays(-6)),
                new SeedProduct("Surface Cleaner", "Multi-surface disinfectant spray", 5.99m, 11, "Cleaning", "Nova Supply Co.", DateTime.UtcNow.AddDays(-4))
            };

            var existingNames = context.Products.Select(product => product.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var newProducts = new List<Product>();

            foreach (var seed in seedProducts)
            {
                if (existingNames.Contains(seed.Name))
                {
                    continue;
                }

                if (!categories.TryGetValue(seed.CategoryName, out var categoryId) || !suppliers.TryGetValue(seed.SupplierName, out var supplierId))
                {
                    continue;
                }

                newProducts.Add(new Product
                {
                    Name = seed.Name,
                    Description = seed.Description,
                    Price = seed.Price,
                    Quantity = seed.Quantity,
                    CategoryId = categoryId,
                    SupplierId = supplierId,
                    CreatedAt = seed.CreatedAt
                });
            }

            if (newProducts.Count > 0)
            {
                context.Products.AddRange(newProducts);
                context.SaveChanges();
            }
        }

        private static void SeedInvoices(ApplicationDbContext context)
        {
            var productsByName = context.Products.ToDictionary(product => product.Name, product => product, StringComparer.OrdinalIgnoreCase);

            var invoiceTemplates = new[]
            {
                new SeedInvoice("Walk-in Customer", DateTime.UtcNow.AddDays(-7), new[]
                {
                    new SeedInvoiceLine("Wireless Mouse", 2),
                    new SeedInvoiceLine("A4 Copy Paper Pack", 4)
                }),
                new SeedInvoice("Apex Legal Ltd.", DateTime.UtcNow.AddDays(-5), new[]
                {
                    new SeedInvoiceLine("Mechanical Keyboard", 3),
                    new SeedInvoiceLine("Whiteboard Markers", 10),
                    new SeedInvoiceLine("Stapler", 2)
                }),
                new SeedInvoice("CityNet Services", DateTime.UtcNow.AddDays(-3), new[]
                {
                    new SeedInvoiceLine("Wi-Fi Router AX3000", 2),
                    new SeedInvoiceLine("HDMI Cable 2m", 6)
                }),
                new SeedInvoice("Bright School", DateTime.UtcNow.AddDays(-1), new[]
                {
                    new SeedInvoiceLine("Desk Lamp", 3),
                    new SeedInvoiceLine("Surface Cleaner", 8),
                    new SeedInvoiceLine("A4 Copy Paper Pack", 12)
                })
            };

            foreach (var template in invoiceTemplates)
            {
                var exists = context.Invoices.Any(invoice =>
                    invoice.CustomerName == template.CustomerName &&
                    invoice.InvoiceDate.Date == template.InvoiceDate.Date);

                if (exists)
                {
                    continue;
                }

                var invoiceItems = new List<InvoiceItem>();
                foreach (var line in template.Lines)
                {
                    if (!productsByName.TryGetValue(line.ProductName, out var product))
                    {
                        continue;
                    }

                    invoiceItems.Add(new InvoiceItem
                    {
                        ProductId = product.Id,
                        Quantity = line.Quantity,
                        Price = product.Price
                    });
                }

                if (invoiceItems.Count == 0)
                {
                    continue;
                }

                var invoice = new Invoice
                {
                    CustomerName = template.CustomerName,
                    InvoiceDate = template.InvoiceDate,
                    Items = invoiceItems,
                    TotalAmount = invoiceItems.Sum(item => item.Price * item.Quantity)
                };

                context.Invoices.Add(invoice);
            }

            context.SaveChanges();
        }

        private static bool ColumnExists(ApplicationDbContext context, string tableName, string columnName)
        {
            using var connection = context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info('{tableName}')";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetString(1) == columnName)
                {
                    return true;
                }
            }

            return false;
        }

        private sealed record SeedProduct(
            string Name,
            string Description,
            decimal Price,
            int Quantity,
            string CategoryName,
            string SupplierName,
            DateTime CreatedAt);

        private sealed record SeedInvoice(string CustomerName, DateTime InvoiceDate, IReadOnlyList<SeedInvoiceLine> Lines);

        private sealed record SeedInvoiceLine(string ProductName, int Quantity);
    }
}
