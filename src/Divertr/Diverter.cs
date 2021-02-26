using System;
using System.Collections.Concurrent;
using System.Reflection;
using DivertR.Internal;

namespace DivertR
{
    public class Diverter : IDiverter
    {
        private readonly RouteRepository _routeRepository = new RouteRepository();
        private readonly ConcurrentDictionary<RouterId, object> _routers = new ConcurrentDictionary<RouterId, object>();

        public IRouter<T> Router<T>(string? name = null) where T : class
        {
            return (IRouter<T>) _routers.GetOrAdd(RouterId.From<T>(name),
                id => new Router<T>(id, _routeRepository));
        }
        
        public IRouter Router(Type type, string? name = null)
        {
            const BindingFlags activatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            return (IRouter) _routers.GetOrAdd(RouterId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Router<>).MakeGenericType(type);
                    return Activator.CreateInstance(diverterType, activatorFlags, null, new object[] {id, _routeRepository}, default);
                });
        }
        
        public IDiverter ResetAll()
        {
            _routeRepository.ResetAll();
            return this;
        }

        public T Proxy<T>(T? original = null) where T : class
        {
            return Router<T>().Proxy(original);
        }

        public IRouter<T> Redirect<T>(T target) where T : class
        {
            return Router<T>().Redirect(target);
        }

        public IRouter<T> AddRedirect<T>(T target) where T : class
        {
            return Router<T>().AddRedirect(target);
        }

        public IRouter<T> Reset<T>() where T : class
        {
            return Router<T>().Reset();
        }

        public ICallRelay<T> Relay<T>() where T : class
        {
            return Router<T>().Relay;
        }
    }
}