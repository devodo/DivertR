using System;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Divert(this IServiceCollection services, IDiverter diverter, string? name = null)
        {
            return services.Divert(diverter, builder =>
            {
                foreach (var viaId in diverter.RegisteredVias(name))
                {
                    builder.Include(viaId.Type).WithViaName(viaId.Name);
                }
            });
        }
        
        private static IServiceCollection Divert(this IServiceCollection services, IDiverter diverter, Action<RegistrationBuilder>? builderAction = null)
        {
            var builder = new RegistrationBuilder(services, diverter);
            builderAction?.Invoke(builder);
            builder.Build().Register();
            return services;
        }

        public static IServiceCollection Divert<TTarget>(this IServiceCollection services, IDiverter diverter, string? name = null) where TTarget : class
        {
            return services.Divert(diverter, typeof(TTarget), name);
        }
        
        public static IServiceCollection Divert(this IServiceCollection services, IDiverter diverter, Type type, string? name = null)
        {
            return services.Divert(diverter, builder =>
            {
                builder.Include(type).WithViaName(name);
            });
        }
        
        public static IServiceCollection Divert(this IServiceCollection services, IVia via)
        {
            var configuration = new RegistrationConfiguration(services, via);
            configuration.IncludeTypes.Add(via.ViaId.Type);
            new DiverterRegistrar(configuration).Register();

            return services;
        }
    }
}
