using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScrummersSalesApi.Backend.Products.Domain.Ports;
using ScrummersSalesApi.Backend.Products.Infrastructure.Adapters;
using ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection svc, IConfiguration config)
        {

            var connectionStringDb = config.GetConnectionString("database");

            svc.AddDbContext<ProductsDbContext>(options =>
                                    options.UseSqlServer(connectionStringDb));

            svc.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return svc;
        }
    }
}
