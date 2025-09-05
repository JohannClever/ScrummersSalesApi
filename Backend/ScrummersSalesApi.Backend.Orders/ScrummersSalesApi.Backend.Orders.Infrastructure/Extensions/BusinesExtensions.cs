using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScrummersSalesApi.Backend.Orders.Domain.Ports;
using ScrummersSalesApi.Backend.Orders.Infrastructure.Adapters.Http;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.Extensions
{
    public static class BusinesExtensions
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>(c =>
            {
                c.BaseAddress = new Uri(configuration["Services:ProductsBaseUrl"]!);
            })
            .AddPolicyHandler(ProductCatalogClient.RetryPolicy)
            .AddPolicyHandler(ProductCatalogClient.CircuitBreakerPolicy)
            .AddPolicyHandler(ProductCatalogClient.TimeoutPolicy);
            return services;
        }
    }
}
