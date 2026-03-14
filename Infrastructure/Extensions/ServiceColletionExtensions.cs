using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infrastructure.Extensions
{
    
    /// <summary>
    /// Métodos de extensión para IServiceCollection
    /// </summary>
    public static class ServiceColletionExtensions
    {
        /// <summary>
        /// Registra automáticamente todos los servicios/repositorios en un ensamblado.
        /// </summary>
        /// <param name="services">Contenedor de servicios</param>
        /// <param name="assembly">Ensamblado a escanear</param>
        /// <exception cref="InvalidOperationException">
        /// El servicio/repositorio debe implementar solo una interfaz que derive de T
        /// </exception>
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
