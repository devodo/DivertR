using System;
using System.Collections.Concurrent;
using System.Reflection;
using DivertR.Core;
using DivertR.DispatchProxy;
using DivertR.Internal;
using DivertR.Setup;

namespace DivertR
{
    public class Diverter : IDiverter
    {
        private readonly RedirectRepository _redirectRepository = new RedirectRepository();
        private readonly ConcurrentDictionary<ViaId, IVia> _vias = new ConcurrentDictionary<ViaId, IVia>();
        private readonly IDiverterSettings _diverterSettings;

        public Diverter(IDiverterSettings? diverterSettings = null)
        {
            _diverterSettings = diverterSettings ?? DiverterSettings.Default;
        }

        public IVia<T> Via<T>(string? name = null) where T : class
        {
            return (IVia<T>) _vias.GetOrAdd(ViaId.From<T>(name),
                id => new Via<T>(id, _redirectRepository, _diverterSettings));
        }
        
        public IVia Via(Type type, string? name = null)
        {
            const BindingFlags activatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            return _vias.GetOrAdd(ViaId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Via<>).MakeGenericType(type);
                    var constructorParams = new object[] {id, _redirectRepository, _diverterSettings};
                    return (IVia) Activator.CreateInstance(diverterType, activatorFlags, null, constructorParams, default);
                });
        }
        
        public IDiverter ResetAll()
        {
            _redirectRepository.ResetAll();
            return this;
        }
    }
}