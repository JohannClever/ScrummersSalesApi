using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess.SeedData
{
    public static class DbSeeder
    {
        // NOTE: async void is generally discouraged; keeping your signature for parity.
        public async static void SeedData(ProductsDbContext dbContext)
        {
            // If your DB uses "Products" (PascalCase) as the actual table name,
            // change this to TableExists("Products")
            bool isTableExists = dbContext.TableExists("products");
            if (!isTableExists) return;

            var existing = dbContext.Set<Product>().ToList();
            if (existing.Any()) return;

            // Fixed IDs for reproducible seeds
            var p1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var p2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var p3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var p4Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var p5Id = Guid.Parse("55555555-5555-5555-5555-555555555555");

            // Use UTC timestamps so data is consistent across environments
            var now = DateTime.UtcNow;

            var products = new List<Product>
            {
                new Product
                {
                    Id = p1Id,
                    Name = "Flights",
                    Description = "International flights",
                    Price = 50m,
                    Stock = 10,
                    CreateDate = now,
                    UpdatedDate = now
                },
                new Product
                {
                    Id = p2Id,
                    Name = "Hotel",
                    Description = "5-star hotel accommodation",
                    Price = 120m,
                    Stock = 5,
                    CreateDate = now,
                    UpdatedDate = now
                },
                new Product
                {
                    Id = p3Id,
                    Name = "Travel Insurance",
                    Description = "International medical coverage",
                    Price = 30m,
                    Stock = 100,
                    CreateDate = now,
                    UpdatedDate = now
                },
                new Product
                {
                    Id = p4Id,
                    Name = "Car Rental",
                    Description = "Compact car rental - 24 hours",
                    Price = 45m,
                    Stock = 25,
                    CreateDate = now,
                    UpdatedDate = now
                },
                new Product
                {
                    Id = p5Id,
                    Name = "City Tour",
                    Description = "Guided city tour (full day)",
                    Price = 80m,
                    Stock = 15,
                    CreateDate = now,
                    UpdatedDate = now
                }
            };

            dbContext.Set<Product>().AddRange(products);
            dbContext.SaveChanges();
        }
    }
}
