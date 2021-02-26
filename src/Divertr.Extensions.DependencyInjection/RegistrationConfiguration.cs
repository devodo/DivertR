using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.Extensions.DependencyInjection
{
    public class RegistrationConfiguration
    {
        public IServiceCollection Services { get; }
        public IDiverter Diverters { get; }

        public HashSet<Type> IncludeTypes { get; } = new HashSet<Type>();
        public HashSet<Type> ExcludeTypes { get; } = new HashSet<Type>();
        
        public string? Name { get; set; }

        public RegistrationConfiguration(IServiceCollection services, IDiverter diverters)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Diverters = diverters ?? throw new ArgumentNullException(nameof(diverters));
        }
    }
}