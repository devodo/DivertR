using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.Extensions.DependencyInjection
{
    public class RegistrationBuilder
    {
        private readonly RegistrationConfiguration _configuration;
        private event EventHandler<IEnumerable<Type>>? TypesDivertedEvent;
        
        public RegistrationBuilder(IServiceCollection services, IDiverter diverters)
        {
            _configuration = new RegistrationConfiguration(services, diverters);
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

            _configuration.IncludeTypes.Add(type);

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
            return Include((IEnumerable<Type>) types);
        }
        
        public RegistrationBuilder IncludeRange<TStartType, TEndType>(bool startInclusive = true, bool endInclusive = true)
        {
            return IncludeRange(typeof(TStartType), typeof(TEndType), startInclusive, endInclusive);
        }
        
        public RegistrationBuilder IncludeFrom<TStartType>(bool inclusive = true)
        {
            return IncludeRange(typeof(TStartType), endType: null, inclusive);
        }
        
        public RegistrationBuilder IncludeUntil<TEndTypeType>(bool inclusive = true)
        {
            return IncludeRange(startType: null, typeof(TEndTypeType), inclusive);
        }
        public RegistrationBuilder IncludeRange(Type? startType = null, Type? endType = null, bool startInclusive = true, bool endInclusive = true)
        {
            foreach (var type in _configuration.Services.GetRange(startType, endType, startInclusive, endInclusive))
            {
                _configuration.IncludeTypes.Add(type);
            }

            return this;
        }
        
        public RegistrationBuilder Exclude<T>() where T : class
        {
            return Exclude(typeof(T));
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

            _configuration.ExcludeTypes.Add(type);

            return this;
        }
        
        public RegistrationBuilder Exclude(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            
            foreach (var type in types)
            {
                Exclude(type);
            }

            return this;
        }
        
        public RegistrationBuilder Exclude(params Type[] types)
        {
            return Exclude((IEnumerable<Type>) types);
        }
        
        public RegistrationBuilder ExcludeRange<TStartType, TEndType>(bool inclusive = true, bool endInclusive = true)
        {
            return ExcludeRange(typeof(TStartType), typeof(TEndType), inclusive, endInclusive);
        }
        
        public RegistrationBuilder ExcludeFrom<TStartType>(bool inclusive = true)
        {
            return ExcludeRange(typeof(TStartType), endType: null, inclusive);
        }
        
        public RegistrationBuilder ExcludeUntil<TEndType>(bool inclusive = true)
        {
            return ExcludeRange(startType: null, typeof(TEndType), inclusive);
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
        
        public RegistrationBuilder WithTypesDivertedHandler(EventHandler<IEnumerable<Type>> handler)
        {
            TypesDivertedEvent += handler;

            return this;
        }

        public DiverterRegistration Build()
        {
            return new DiverterRegistration(_configuration, TypesDivertedEvent);
        } 
    }
}