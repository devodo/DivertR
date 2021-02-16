using System;
using System.Collections.Concurrent;
using System.Reflection;
using Divertr.Internal;

namespace Divertr
{
    public class DiverterCollection : IDiverterCollection
    {
        private readonly RouteRepository _routeRepository = new RouteRepository();
        private readonly ConcurrentDictionary<DiverterId, object> _diversions = new ConcurrentDictionary<DiverterId, object>();
        
        public DiverterCollection() {}

        ffpublic DiverterCollection(IDiverter diverter)
        {
            _diversions[diverter.DiverterId] = diverter;
        }

        public IDiverter<T> Of<T>(string? name = null) where T : class
        {
            return (IDiverter<T>) _diversions.GetOrAdd(DiverterId.From<T>(name),
                id => new Diverter<T>(id, _routeRepository));
        }
        
        public IDiverter Of(Type type, string? name = null)
        {
            const BindingFlags activatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            return (IDiverter) _diversions.GetOrAdd(DiverterId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Diverter<>).MakeGenericType(type);
                    return Activator.CreateInstance(diverterType, activatorFlags, null, new object[] {id, _routeRepository}, default);
                });
        }
        
        public IDiverterCollection ResetAll()
        {
            _routeRepository.ResetAll();
            return this;
        }
    }
}