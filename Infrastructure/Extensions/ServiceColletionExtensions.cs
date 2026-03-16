using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infrastructure.Extensions
{
    public static class ServiceColletionExtensions
    {
        public static void AddMappedComponents<T>(this IServiceCollection services, Assembly assembly)
        {
            var baseType = typeof(T);

            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));

            foreach (var implementationType in types)
            {
                var specificInterface = implementationType.GetInterfaces()
                    .FirstOrDefault(i => i != baseType && baseType.IsAssignableFrom(i));

                if (specificInterface != null)
                {
                    services.AddScoped(specificInterface, implementationType);
                }
            }
        }
    }
}
