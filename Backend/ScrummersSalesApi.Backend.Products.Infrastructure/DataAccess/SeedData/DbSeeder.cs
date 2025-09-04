using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess.SeedData
{
    public static class DbSeeder
    {
        public async static void SeedData(ProductsDbContext dbContext)
        {
            bool isTableExists = dbContext.TableExists("products");
            if (isTableExists)
            {
                var services = dbContext.Set<Product>().ToList();
                if (!services.Any())
                {
                    dbContext.Set<Product>().Add(new Product()
                    {
                        Description = "Vuelos internacionales",
                        Name = "Vuelos",
                        Price =50,
                        Stock = 10
                    });
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
