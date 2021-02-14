using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Divertr.Internal;

namespace Divertr
{
    public class Diverter : IDiverter
    {
        private readonly DiverterState _diverterState = new DiverterState();
        private readonly ConcurrentDictionary<DiversionId, object> _diversions = new ConcurrentDictionary<DiversionId, object>();

        public IDiversion<T> Of<T>(string? name = null) where T : class
        {
            return (IDiversion<T>) _diversions.GetOrAdd(DiversionId.From<T>(name),
                id => new Diversion<T>(id, _diverterState));
        }
        
        public IDiversion Of(Type type, string? name = null)
        {
            const BindingFlags activatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            return (IDiversion) _diversions.GetOrAdd(DiversionId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Diversion<>).MakeGenericType(type);
                    return Activator.CreateInstance(diverterType, activatorFlags, null, new object[] {id, _diverterState}, default);
                });
        }
        
        public IDiverter ResetAll()
        {
            _diverterState.ResetAll();
            return this;
        }
        
        public IEnumerable<Type> KnownTypes(string? name = null)
        {
            return _diversions.Keys
                .Where(diverterId => name == null || diverterId.Name == name)
                .Select(diverterId => diverterId.Type);
        }
    }
}