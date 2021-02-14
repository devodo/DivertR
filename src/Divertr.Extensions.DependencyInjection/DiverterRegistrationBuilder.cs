using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Divertr.Extensions.DependencyInjection
{
    public class DiverterRegistrationBuilder
    {
        private readonly RegistrationConfiguration _configuration;
        private event EventHandler<IEnumerable<Type>>? TypesRegistered;
        
        public DiverterRegistrationBuilder(IServiceCollection services, IDiverter diverter)
        {
            _configuration = new RegistrationConfiguration(services, diverter);
        }

        public DiverterRegistrationBuilder Include<T>() where T : class
        {
            return Include(typeof(T));
        }
        
        public DiverterRegistrationBuilder Include(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            
            if (!type.IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", type.Name);
            }

            if (type.ContainsGenericParameters)
            {
                throw new ArgumentException("Only closed generic types are supported", type.Name);
            }
            
            if (!_configuration.IncludeTypes.Add(type))
            {
                throw new DiverterException($"Type {type} already included");
            }

            return this;
        }

        public DiverterRegistrationBuilder Include(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            
            foreach (var type in types)
            {
                Include(type);
            }

            return this;
        }
        
        public DiverterRegistrationBuilder Include(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            
            foreach (var type in types)
            {
                Include(type);
            }

            return this;
        }
        
        public DiverterRegistrationBuilder IncludeRange<TStartType, TEndType>(bool startInclusive = true, bool endInclusive = true)
        {
            return IncludeRange(typeof(TStartType), typeof(TEndType), startInclusive, endInclusive);
        }
        
        public DiverterRegistrationBuilder IncludeRange<TStartType>(bool startInclusive = true)
        {
            return IncludeRange(typeof(TStartType), endType: null, startInclusive);
        }

        public DiverterRegistrationBuilder IncludeRange(Type? startType = null, Type? endType = null, bool startInclusive = true, bool endInclusive = true)
        {
            foreach (var type in _configuration.Services.GetRange(startType, endType, startInclusive, endInclusive))
            {
                _configuration.IncludeTypes.Add(type);
            }

            return this;
        }
        
        public DiverterRegistrationBuilder Exclude(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            
            if (!type.IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", type.Name);
            }

            if (type.ContainsGenericParameters)
            {
                throw new ArgumentException("Only closed generic types are supported", type.Name);
            }
            
            if (!_configuration.ExcludeTypes.Add(type))
            {
                throw new DiverterException($"Type {type} already excluded");
            }

            return this;
        }
        
        public DiverterRegistrationBuilder ExcludeRange<TStartType, TEndType>(bool startInclusive = true, bool endInclusive = true)
        {
            return ExcludeRange(typeof(TStartType), typeof(TEndType), startInclusive, endInclusive);
        }
        
        public DiverterRegistrationBuilder ExcludeRange<TStartType>(bool startInclusive = true)
        {
            return ExcludeRange(typeof(TStartType), endType: null, startInclusive);
        }

        public DiverterRegistrationBuilder ExcludeRange(Type? startType = null, Type? endType = null, bool startInclusive = true, bool endInclusive = true)
        {
            foreach (var type in _configuration.Services.GetRange(startType, endType, startInclusive, endInclusive))
            {
                _configuration.ExcludeTypes.Add(type);
            }

            return this;
        }

        public DiverterRegistrationBuilder WithName(string? name)
        {
            _configuration.Name = name;

            return this;
        }
        
        public DiverterRegistrationBuilder WithTypesRegisteredHandler(EventHandler<IEnumerable<Type>> typesRegisteredHandler)
        {
            TypesRegistered += typesRegisteredHandler;

            return this;
        }

        public DiverterRegistration Build()
        {
            return new DiverterRegistration(_configuration, TypesRegistered);
        } 
    }
}