using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DivertR
{
    /// <inheritdoc />
    public class Diverter : IDiverter
    {
        private readonly ConcurrentDictionary<ViaId, IVia> _registeredVias = new ConcurrentDictionary<ViaId, IVia>();
        private readonly IViaSet _viaSet;

        /// <summary>
        /// Create a <see cref="Diverter"/> instance.
        /// </summary>
        /// <param name="settings">Optionally override default DivertR settings.</param>
        public Diverter(DiverterSettings? settings = null)
        {
            _viaSet = new ViaSet(settings);
        }

        public IDiverter Register<TTarget>(string? name = null) where TTarget : class
        {
            var via = _viaSet.Via<TTarget>(name);

            if (!_registeredVias.TryAdd(via.ViaId, via))
            {
                throw new DiverterException($"Via already registered for {via.ViaId}");
            }

            return this;
        }
        
        public IDiverter Register(Type targetType, string? name = null)
        {
            var via = _viaSet.Via(targetType, name);
            
            if (!_registeredVias.TryAdd(via.ViaId, via))
            {
                throw new DiverterException($"Via already registered for {via.ViaId}");
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
            name ??= string.Empty;
            
            return _registeredVias
                .Where(x => x.Key.Name == name)
                .Select(x => x.Value);
        }
        
        public IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class
        {
            return (IVia<TTarget>) Via(ViaId.From<TTarget>(name));
        }

        public IVia Via(Type targetType, string? name = null)
        {
            return Via(ViaId.From(targetType, name));
        }
        
        public IVia Via(ViaId id)
        {
            if (!_registeredVias.TryGetValue(id, out var via))
            {
                throw new DiverterException($"Via not registered for {id}");
            }
            
            return via;
        }
        
        public IDiverter StrictAll()
        {
            _viaSet.StrictAll();
            
            return this;
        }


        public IDiverter Strict(string? name = null)
        {
            _viaSet.Strict(name);

            return this;
        }
        
        public IDiverter ResetAll()
        {
            _viaSet.ResetAll();
            
            return this;
        }

        public IDiverter Reset(string? name = null)
        {
            _viaSet.Reset(name);

            return this;
        }
    }
}