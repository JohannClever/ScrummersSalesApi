using Microsoft.Extensions.DependencyInjection;
using ScrummersSalesApi.Backend.Orders.Infrastructure.Authorization;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.Extensions
{
    public static  class JwtExtension
    {
        public static IServiceCollection AddJwtServices(this IServiceCollection svc)
        {
            svc.AddScoped<JwtValidationService>();
            svc.AddScoped<JwtService>();
            return svc;
            
        }
    }
}
