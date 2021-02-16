using System;
using System.Collections.Concurrent;

namespace Divertr.Internal
{
    internal class RouteRepository
    {
        private readonly ConcurrentDictionary<DiverterId, object> _routes = new ConcurrentDictionary<DiverterId, object>();

        public DiversionRoute<T>? GetRoute<T>(DiverterId diverterId) where T : class
        {
            if (_routes.TryGetValue(diverterId, out var alteration))
            {
                return (DiversionRoute<T>)alteration;
            }

            return null;
        }

        public DiversionRoute<T> AddOrUpdateRoute<T>(DiverterId diverterId, Func<DiversionRoute<T>> addFactory, Func<DiversionRoute<T>, DiversionRoute<T>> updateFactory) where T : class
        {
            object Create(DiverterId _)
            {
                return addFactory.Invoke();
            }

            object Update(DiverterId _, object existing)
            {
                return updateFactory((DiversionRoute<T>) existing);
            }

            return (DiversionRoute<T>)_routes.AddOrUpdate(diverterId, Create, Update);
        }

        public void SetRoute<T>(DiverterId diverterId, DiversionRoute<T> diversionRoute) where T : class
        {
            _routes[diverterId] = diversionRoute;
        }

        public bool Reset(DiverterId diverterId)
        {
            return _routes.TryRemove(diverterId, out _);
        }
        
        public void ResetAll()
        {
            _routes.Clear();
        }
    }
}