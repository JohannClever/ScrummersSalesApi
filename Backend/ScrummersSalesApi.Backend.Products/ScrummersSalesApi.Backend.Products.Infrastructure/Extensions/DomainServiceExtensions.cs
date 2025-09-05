using Microsoft.Extensions.DependencyInjection;
using ScrummersSalesApi.Backend.Products.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.Extensions
{
    public static class DomainServiceExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection svc)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var domainServices = assemblies.SelectMany(a => a.GetTypes())
                                           .Where(t => t.GetCustomAttribute<DomainServiceAttribute>() != null);

            // Registra los servicios y sus interfaces en el contenedor de servicios
            foreach (var serviceType in domainServices)
            {
                var interfaceType = serviceType.GetInterfaces().FirstOrDefault();
                if (interfaceType != null)
                {
                    svc.AddTransient(interfaceType, serviceType);
                }
            }
            return svc;
        }
    }
}
