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
        private readonly ConcurrentDictionary<DiverterId, object> _directors = new ConcurrentDictionary<DiverterId, object>();

        public IDiversion<T> Of<T>(string? name = null) where T : class
        {
            return (IDiversion<T>) _directors.GetOrAdd(DiverterId.From<T>(name),
                id => new Diversion<T>(id, _diverterState));
        }
        
        public IDiversion Of(Type type, string? name = null)
        {
            const BindingFlags activatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            return (IDiversion) _directors.GetOrAdd(DiverterId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Diversion<>).MakeGenericType(type);
                    return Activator.CreateInstance(diverterType, activatorFlags, null, new object[] {id, _diverterState}, default);
                });
        }
        
        public IDiversion<T> Redirect<T>(T target, string? name = null) where T : class
        {
            return Of<T>(name).Redirect(target);
        }

        public IDiversion<T> Reset<T>(string? name = null) where T : class
        {
            return Of<T>(name).Reset();
        }

        public IDiverter ResetAll()
        {
            _diverterState.Reset();
            return this;
        }
        
        public T Proxy<T>(T? origin = null, string? name = null) where T : class
        {
            return Of<T>(name).Proxy(origin);
        }

        public ICallContext<T> CallCtx<T>(string? name = null) where T : class
        {
            return Of<T>(name).CallCtx;
        }

        public IEnumerable<Type> KnownTypes(string? name = null)
        {
            return _directors.Keys
                .Where(diverterId => name == null || diverterId.Name == name)
                .Select(diverterId => diverterId.Type);
        }
    }
}