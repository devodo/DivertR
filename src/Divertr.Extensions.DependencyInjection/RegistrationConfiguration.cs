using System;
using System.Collections.Generic;
using DivertR.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.Extensions.DependencyInjection
{
    public class RegistrationConfiguration
    {
        public IServiceCollection Services { get; }

        public Func<Type, IVia> GetRouterFunc { get; } 

        public HashSet<Type> IncludeTypes { get; } = new HashSet<Type>();
        public HashSet<Type> ExcludeTypes { get; } = new HashSet<Type>();
        
        public string? Name { get; set; }

        public RegistrationConfiguration(IServiceCollection services, IDiverter diverter)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            if (diverter == null) throw new ArgumentNullException(nameof(diverter));
            
            GetRouterFunc = type => diverter.Via(type, Name);
        }
        
        public RegistrationConfiguration(IServiceCollection services, IVia via)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            if (via == null) throw new ArgumentNullException(nameof(via));

            GetRouterFunc = type =>
            {
                if (type != via.ViaId.Type)
                {
                    throw new InvalidOperationException($"Include type {type} does not matcher router type {via.ViaId.Type}");
                }

                return via;
            };
        }
    }
}