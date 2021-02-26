using System;
using System.Collections.Concurrent;

namespace DivertR.Internal
{
    internal class RouteRepository
    {
        private readonly ConcurrentDictionary<RouterId, object> _routes = new ConcurrentDictionary<RouterId, object>();

        public RedirectRoute<T>? GetRoute<T>(RouterId routerId) where T : class
        {
            if (_routes.TryGetValue(routerId, out var alteration))
            {
                return (RedirectRoute<T>)alteration;
            }

            return null;
        }

        public RedirectRoute<T> AddOrUpdateRoute<T>(RouterId routerId, Func<RedirectRoute<T>> addFactory, Func<RedirectRoute<T>, RedirectRoute<T>> updateFactory) where T : class
        {
            object Create(RouterId _)
            {
                return addFactory.Invoke();
            }

            object Update(RouterId _, object existing)
            {
                return updateFactory((RedirectRoute<T>) existing);
            }

            return (RedirectRoute<T>)_routes.AddOrUpdate(routerId, Create, Update);
        }

        public void SetRoute<T>(RouterId routerId, RedirectRoute<T> redirectRoute) where T : class
        {
            _routes[routerId] = redirectRoute;
        }

        public bool Reset(RouterId routerId)
        {
            return _routes.TryRemove(routerId, out _);
        }
        
        public void ResetAll()
        {
            _routes.Clear();
        }
    }
}