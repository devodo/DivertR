using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Divertr.Extensions.DependencyInjection
{
    public class RegistrationBuilder
    {
        private readonly RegistrationConfiguration _configuration;
        private event EventHandler<IEnumerable<Type>>? TypesRegistered;
        
        public RegistrationBuilder(IServiceCollection services, IDiverter diverter)
        {
            _configuration = new RegistrationConfiguration(services, diverter);
        }

        public RegistrationBuilder Include<T>() where T : class
        {
            return Include(typeof(T));
        }
        
        public RegistrationBuilder Include(Type type)
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

        public RegistrationBuilder Include(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            
            foreach (var type in types)
            {
                Include(type);
            }

            return this;
        }
        
        public RegistrationBuilder Include(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            
            foreach (var type in types)
            {
                Include(type);
            }

            return this;
        }
        
        public RegistrationBuilder IncludeRange<TStartType, TEndType>(bool startInclusive = true, bool endInclusive = true)
        {
            return IncludeRange(typeof(TStartType), typeof(TEndType), startInclusive, endInclusive);
        }
        
        public RegistrationBuilder IncludeRange<TStartType>(bool startInclusive = true)
        {
            return IncludeRange(typeof(TStartType), endType: null, startInclusive);
        }

        public RegistrationBuilder IncludeRange(Type? startType = null, Type? endType = null, bool startInclusive = true, bool endInclusive = true)
        {
            foreach (var type in _configuration.Services.GetRange(startType, endType, startInclusive, endInclusive))
            {
                _configuration.IncludeTypes.Add(type);
            }

            return this;
        }
        
        public RegistrationBuilder Exclude(Type type)
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
        
        public RegistrationBuilder ExcludeRange<TStartType, TEndType>(bool startInclusive = true, bool endInclusive = true)
        {
            return ExcludeRange(typeof(TStartType), typeof(TEndType), startInclusive, endInclusive);
        }
        
        public RegistrationBuilder ExcludeRange<TStartType>(bool startInclusive = true)
        {
            return ExcludeRange(typeof(TStartType), endType: null, startInclusive);
        }

        public RegistrationBuilder ExcludeRange(Type? startType = null, Type? endType = null, bool startInclusive = true, bool endInclusive = true)
        {
            foreach (var type in _configuration.Services.GetRange(startType, endType, startInclusive, endInclusive))
            {
                _configuration.ExcludeTypes.Add(type);
            }

            return this;
        }

        public RegistrationBuilder WithName(string? name)
        {
            _configuration.Name = name;

            return this;
        }
        
        public RegistrationBuilder WithTypesRegisteredHandler(EventHandler<IEnumerable<Type>> typesRegisteredHandler)
        {
            TypesRegistered += typesRegisteredHandler;

            return this;
        }

        public Registration Build()
        {
            return new Registration(_configuration, TypesRegistered);
        } 
    }
}