using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScrummersSalesApi.Backend.Orders.Domain.Ports;
using ScrummersSalesApi.Backend.Orders.Infrastructure.Adapters;
using ScrummersSalesApi.Backend.Orders.Infrastructure.DataAccess;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection svc, IConfiguration config)
        {

            var connectionStringDb = config.GetConnectionString("database");

            svc.AddDbContext<OrdersDbContext>(options =>
                                    options.UseSqlServer(connectionStringDb));

            svc.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return svc;
        }
    }
}
