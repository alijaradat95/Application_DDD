using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;

namespace Application.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a =>
                    !a.IsDynamic &&
                    (a.FullName.StartsWith("Application.Application") ||
                     a.FullName.StartsWith("Application.Contracts")))
                .ToList();

            var contractsAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Application.Contracts")
                ?? Assembly.Load("Application.Contracts");

            var implementationAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Application.Application")
                ?? Assembly.Load("Application.Application");

            if (contractsAssembly == null || implementationAssembly == null)
                throw new Exception("Contracts or implementation assembly not found");

            var contractTypes = contractsAssembly.GetTypes()
                .Where(t => t.IsInterface && t.Name.EndsWith("AppService"))
                .ToList();

            var implTypes = implementationAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .ToList();

            foreach (var contract in contractTypes)
            {
                var impl = implTypes.FirstOrDefault(c => contract.IsAssignableFrom(c));
                if (impl != null)
                {
                    services.AddScoped(contract, impl);
                }
            }

            return services;
        }
    }
}
