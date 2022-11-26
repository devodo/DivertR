using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace DivertR
{
    /// <inheritdoc />
    public class ViaSet : IViaSet
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, IVia>> _viaGroups = new();
        
        public ViaSet(DiverterSettings? settings = null)
        {
            Settings = settings ?? DiverterSettings.Global;
        }

        public DiverterSettings Settings { get; }

        public IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class?
        {
            var viaId = ViaId.From<TTarget>(name);
            var viaGroup = GetViaGroup(viaId.Name);
            var via = viaGroup.GetOrAdd(viaId.Type, _ => new Via<TTarget>(viaId, this));

            return (IVia<TTarget>) via;
        }
        
        public IVia Via(Type targetType, string? name = null)
        {
            var viaId = ViaId.From(targetType, name);
            
            IVia CreateVia(Type type)
            {
                const BindingFlags ActivatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                
                var diverterType = typeof(Via<>).MakeGenericType(type);
                var constructorParams = new object[] { viaId, this, null! };
                var via = (IVia) Activator.CreateInstance(diverterType, ActivatorFlags, null, constructorParams, default);

                return via;
            }
            
            var viaGroup = GetViaGroup(viaId.Name);
            
            return viaGroup.GetOrAdd(viaId.Type, CreateVia);
        }

        public IViaSet Reset(string? name = null, bool includePersistent = false)
        {
            var viaGroup = GetViaGroup(name);
            foreach (var via in viaGroup.Values)
            {
                via.Reset(includePersistent);
            }

            return this;
        }

        public IViaSet ResetAll(bool includePersistent = false)
        {
            foreach (var viaGroup in _viaGroups.Values)
            {
                foreach (var via in viaGroup.Values)
                {
                    via.Reset(includePersistent);
                }
            }

            return this;
        }

        public IViaSet Strict(string? name = null)
        {
            var viaGroup = GetViaGroup(name);
            foreach (var via in viaGroup.Values)
            {
                via.Strict();
            }

            return this;
        }

        public IViaSet StrictAll()
        {
            foreach (var viaGroup in _viaGroups.Values)
            {
                foreach (var via in viaGroup.Values)
                {
                    via.Strict();
                }
            }

            return this;
        }
        
        /// <summary>
        /// Add a <see cref="IVia{TTarget}"/> to this <see cref="IViaSet"/>. For internal use by <see cref="IVia{TTarget}"/> constructor.
        /// </summary>
        /// <param name="via">The <see cref="IVia{TTarget}"/> instance to add.</param>
        /// <exception cref="DiverterException">Thrown if <see cref="IVia{TTarget}"/> already exists in this <see cref="IViaSet"/></exception>
        internal void AddVia(IVia via)
        {
            var viaGroup = GetViaGroup(via.ViaId.Name);
            if (!viaGroup.TryAdd(via.ViaId.Type, via))
            {
                throw new DiverterException("Via already exists in ViaSet");
            }
        }

        private ConcurrentDictionary<Type, IVia> GetViaGroup(string? name = null)
        {
            return _viaGroups.GetOrAdd(name ?? string.Empty, _ => new ConcurrentDictionary<Type, IVia>());
        }
    }
}