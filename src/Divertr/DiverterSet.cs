using System;
using System.Collections.Concurrent;
using System.Reflection;
using Divertr.Internal;

namespace Divertr
{
    public class DiverterSet : IDiverterSet
    {
        private readonly DiverterState _diverterState = new DiverterState();
        private readonly ConcurrentDictionary<DiverterId, object> _diverters = new ConcurrentDictionary<DiverterId, object>();

        public T Proxy<T>(T? origin = null, string? name = null) where T : class
        {
            return Get<T>(name).Proxy(origin);
        }

        public IDiverter<T> Get<T>(string? name = null) where T : class
        {
            return (IDiverter<T>) _diverters.GetOrAdd(DiverterId.From<T>(name),
                id => new Diverter<T>(id, _diverterState));
        }

        public IDiverter<T> Redirect<T>(T target, string? name = null) where T : class
        {
            return Get<T>(name).Redirect(target);
        }

        public ICallContext<T> CallCtx<T>(string? name = null) where T : class
        {
            return Get<T>(name).CallCtx;
        }

        public IDiverter Get(Type type, string? name = null)
        {
            const BindingFlags activatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            return (IDiverter) _diverters.GetOrAdd(DiverterId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Diverter<>).MakeGenericType(type);
                    return Activator.CreateInstance(diverterType, activatorFlags, null, new object[] {id, _diverterState}, default);
                });
        }

        public void ResetAll()
        {
            _diverterState.Reset();
        }
    }
}