using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.HttpApi.Host
{
    public static class AppServiceMetadataHelper
    {
        public static IEnumerable<(string Route, MethodInfo Method, Type InterfaceType)> DiscoverAppServices()
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

}
