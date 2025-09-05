using Microsoft.EntityFrameworkCore;
using ScrummersSalesApi.Backend.Orders.Infrastructure.DataAccess.Configuration.Orders;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.DataAccess
{



    public class OrdersDbContext : DbContext
    {
        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

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

            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new OrderItemConfiguration());


        }
    }
}
