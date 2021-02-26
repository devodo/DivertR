using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.Extensions.DependencyInjection
{
    public class RegistrationConfiguration
    {
        public IServiceCollection Services { get; }

        public Func<Type, IRouter> GetRouterFunc { get; } 

        public HashSet<Type> IncludeTypes { get; } = new HashSet<Type>();
        public HashSet<Type> ExcludeTypes { get; } = new HashSet<Type>();
        
        public string? Name { get; set; }

        public RegistrationConfiguration(IServiceCollection services, IDiverter diverter)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            if (diverter == null) throw new ArgumentNullException(nameof(diverter));
            
            GetRouterFunc = type => diverter.Router(type, Name);
        }
        
        public RegistrationConfiguration(IServiceCollection services, IRouter router)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            if (router == null) throw new ArgumentNullException(nameof(router));

            GetRouterFunc = type =>
            {
                if (type != router.RouterId.Type)
                {
                    throw new InvalidOperationException($"Include type {type} does not matcher router type {router.RouterId.Type}");
                }

                return router;
            };
        }
    }
}