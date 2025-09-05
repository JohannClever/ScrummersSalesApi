using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IHostApplicationBuilder AddHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                            .AddSqlServer(
                                                connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
                                                name: "sql",
                                                failureStatus: HealthStatus.Unhealthy
                                            );
            return builder;
        }
    }
}
