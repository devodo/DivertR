using System;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Divert(this IServiceCollection services, IDiverter diverter, Action<RegistrationBuilder>? configureBuilder = null)
        {
            var builder = new RegistrationBuilder(services, diverter);
            configureBuilder?.Invoke(builder);
            builder.Build().Register();
            return services;
        }

        public static IServiceCollection Divert<T>(this IServiceCollection services, IDiverter diverter, string? name = null) where T : class
        {
            return services.Divert(diverter, typeof(T), name);
        }
        
        public static IServiceCollection Divert(this IServiceCollection services, IDiverter diverter, Type type, string? name = null)
        {
            return services.Divert(diverter, builder =>
            {
                builder.Include(type).WithName(name);
            });
        }
        
        public static IServiceCollection Divert(this IServiceCollection services, IRouter router)
        {
            var configuration = new RegistrationConfiguration(services, router);
            configuration.IncludeTypes.Add(router.RouterId.Type);
            new DiverterRegistrar(configuration, null).Register();

            return services;
        }
    }
}