using Microsoft.EntityFrameworkCore;
using ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess.Configuration.Products;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess
{



    public class ProductsDbContext : DbContext
    {
        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options) { }

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ProductConfiguration());


        }
    }
}
