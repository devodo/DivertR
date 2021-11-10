using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DivertR.Internal;
using DivertR.Setup;

namespace DivertR
{
    /// <inheritdoc />
    public class Diverter : IDiverter
    {
        private readonly RedirectRepository _redirectRepository = new RedirectRepository();
        private readonly ConcurrentDictionary<ViaId, IVia> _vias = new ConcurrentDictionary<ViaId, IVia>();
        private readonly IDiverterSettings _diverterSettings;
        
        /// <summary>
        /// Create a <see cref="Diverter"/> instance.
        /// </summary>
        /// <param name="diverterSettings">Optionally override default DivertR settings.</param>
        public Diverter(IDiverterSettings? diverterSettings = null)
        {
            _diverterSettings = diverterSettings ?? DiverterSettings.Default;
        }

        public IDiverter Register<TTarget>(string? name = null) where TTarget : class
        {
            var id = ViaId.From<TTarget>(name);
            
            if (!_vias.TryAdd(id, new Via<TTarget>(id, _redirectRepository, _diverterSettings)))
            {
                throw new DiverterException($"Via already registered for {id}");
            }

            return this;
        }
        
        public IDiverter Register(Type type, string? name = null)
        {
            const BindingFlags ActivatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            var id = ViaId.From(type, name);
            var diverterType = typeof(Via<>).MakeGenericType(type);
            var constructorParams = new object[] { id, _redirectRepository, _diverterSettings };
            var via = (IVia) Activator.CreateInstance(diverterType, ActivatorFlags, null, constructorParams, default);

            if (!_vias.TryAdd(id, via))
            {
                throw new DiverterException($"Via already registered for {id}");
            }

            return this;
        }

        public IDiverter Register(IEnumerable<Type> types, string? name = null)
        {
            foreach (var type in types)
            {
                Register(type, name);
            }

            return this;
        }
        
        public IEnumerable<IVia> RegisteredVias(string? name = null)
        {
            return _vias
                .Where(x => x.Key.Name == name)
                .Select(x => x.Value);
        }
        
        public IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class
        {
            return (IVia<TTarget>) Via(ViaId.From<TTarget>(name));
        }

        public IVia Via(Type type, string? name = null)
        {
            return Via(ViaId.From(type, name));
        }
        
        public IVia Via(ViaId id)
        {
            if (!_vias.TryGetValue(id, out var via))
            {
                throw new DiverterException($"Via not registered for {id}");
            }
            
            return via;
        }
        
        public IDiverter StrictAll()
        {
            foreach (var via in _vias)
            {
                via.Value.Strict();
            }
            
            return this;
        }


        public IDiverter Strict(string? name = null)
        {
            foreach (var via in RegisteredVias(name))
            {
                via.Strict();
            }

            return this;
        }
        
        public IDiverter ResetAll()
        {
            _redirectRepository.ResetAll();
            return this;
        }

        public IDiverter Reset(string? name = null)
        {
            foreach (var via in RegisteredVias(name))
            {
                via.Reset();
            }

            return this;
        }
    }
}