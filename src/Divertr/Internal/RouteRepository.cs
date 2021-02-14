using System;
using System.Collections.Concurrent;

namespace Divertr.Internal
{
    internal class RouteRepository
    {
        private readonly ConcurrentDictionary<DiversionId, object> _routes = new ConcurrentDictionary<DiversionId, object>();

        public DiversionRoute<T>? GetRoute<T>(DiversionId diversionId) where T : class
        {
            if (_routes.TryGetValue(diversionId, out var alteration))
            {
                return (DiversionRoute<T>)alteration;
            }

            return null;
        }

        public DiversionRoute<T> AddOrUpdateRoute<T>(DiversionId diversionId, Func<DiversionRoute<T>> addFactory, Func<DiversionRoute<T>, DiversionRoute<T>> updateFactory) where T : class
        {
            object Create(DiversionId _)
            {
                return addFactory.Invoke();
            }

            object Update(DiversionId _, object existing)
            {
                return updateFactory((DiversionRoute<T>) existing);
            }

            return (DiversionRoute<T>)_routes.AddOrUpdate(diversionId, Create, Update);
        }

        public void SetRoute<T>(DiversionId diversionId, DiversionRoute<T> diversionRoute) where T : class
        {
            _routes[diversionId] = diversionRoute;
        }

        public bool Reset(DiversionId diversionId)
        {
            return _routes.TryRemove(diversionId, out _);
        }
        
        public void ResetAll()
        {
            _routes.Clear();
        }
    }
}