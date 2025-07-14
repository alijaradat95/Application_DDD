using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace Application.HttpApi.Extensions;

public static class AppServiceMetadata
{
    public static IEnumerable<(string Route, MethodInfo Method, Type InterfaceType)> Discover()
    {
        var contractsAssembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Application.Contracts");

        if (contractsAssembly == null)
            yield break;

        var interfaces = contractsAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("AppService"));

        foreach (var iface in interfaces)
        {
            var routePrefix = $"/api/{iface.Name.Replace("I", "").Replace("AppService", "").ToLower()}";

            foreach (var method in iface.GetMethods())
            {
                var route = $"{routePrefix}/{method.Name.Replace("Async", "").ToLower()}";
                yield return (route, method, iface);
            }
        }
    }
}
