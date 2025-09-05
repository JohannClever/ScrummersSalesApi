using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IHostApplicationBuilder AddHealthChecks(this IHostApplicationBuilder builder, string? productsHealthUrl)
        {
            builder.Services.AddHealthChecks()
                            .AddSqlServer(
                                                connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
                                                name: "sql",
                                                failureStatus: HealthStatus.Unhealthy
                                            );

            if (string.IsNullOrWhiteSpace(productsHealthUrl))
                throw new InvalidOperationException("Missing configuration key: Services:ProductsHealthUrl");


            builder.Services
                .AddHealthChecks()
                    .AddUrlGroup(
                        uri: new Uri(productsHealthUrl),
                        name: "order-service",
                        failureStatus: HealthStatus.Unhealthy,
                        tags: new[] { "deps", "http", "order" },
                        timeout: TimeSpan.FromSeconds(5));
            return builder;
        }
    }
}
