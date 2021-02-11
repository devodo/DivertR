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

        public T Proxy<T>(T? origin = null, string? name = null) where T : class
        {
            return For<T>(name).Proxy(origin);
        }

        public IDirector<T> For<T>(string? name = null) where T : class
        {
            return (IDirector<T>) _directors.GetOrAdd(DiverterId.From<T>(name),
                id => new Director<T>(id, _diverterState));
        }

        public IDirector<T> Redirect<T>(T target, string? name = null) where T : class
        {
            return For<T>(name).Redirect(target);
        }

        public ICallContext<T> CallCtx<T>(string? name = null) where T : class
        {
            return For<T>(name).CallCtx;
        }

        public IDirector For(Type type, string? name = null)
        {
            const BindingFlags activatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            return (IDirector) _directors.GetOrAdd(DiverterId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Director<>).MakeGenericType(type);
                    return Activator.CreateInstance(diverterType, activatorFlags, null, new object[] {id, _diverterState}, default);
                });
        }

        public void ResetAll()
        {
            _diverterState.Reset();
        }

        public IEnumerable<Type> KnownTypes(string? name = null)
        {
            return _directors.Keys
                .Where(diverterId => name == null || diverterId.Name == name)
                .Select(diverterId => diverterId.Type);
        }
    }
}