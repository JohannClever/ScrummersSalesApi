using Microsoft.Extensions.DependencyInjection;
using ScrummersSalesApi.Backend.Products.Infrastructure.Authorization;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.Extensions
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
