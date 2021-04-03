using System;
using DivertR.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Divert(this IServiceCollection services, IDiverter diverter, Action<RegistrationBuilder>? builderAction = null)
        {
            var builder = new RegistrationBuilder(services, diverter);
            builderAction?.Invoke(builder);
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
        
        public static IServiceCollection Divert(this IServiceCollection services, IVia via)
        {
            var configuration = new RegistrationConfiguration(services, via);
            configuration.IncludeTypes.Add(via.ViaId.Type);
            new DiverterRegistrar(configuration, null).Register();

            return services;
        }
    }
}