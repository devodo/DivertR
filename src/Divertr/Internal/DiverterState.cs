using System;
using System.Collections.Concurrent;

namespace Divertr.Internal
{
    internal class DiverterState
    {
        private readonly ConcurrentDictionary<DiversionId, object> _callRoutes = new ConcurrentDictionary<DiversionId, object>();

        public DiversionSnapshot<T>? GetCallRoute<T>(DiversionId diversionId) where T : class
        {
            if (_callRoutes.TryGetValue(diversionId, out var alteration))
            {
                return (DiversionSnapshot<T>)alteration;
            }

            return null;
        }

        public DiversionSnapshot<T> AddOrUpdateCallRoute<T>(DiversionId diversionId, Func<DiversionSnapshot<T>> addFactory, Func<DiversionSnapshot<T>, DiversionSnapshot<T>> updateFactory) where T : class
        {
            object Create(DiversionId _)
            {
                return addFactory.Invoke();
            }

            object Update(DiversionId _, object existing)
            {
                return updateFactory((DiversionSnapshot<T>) existing);
            }

            return (DiversionSnapshot<T>)_callRoutes.AddOrUpdate(diversionId, Create, Update);
        }

        public void SetCallRoute<T>(DiversionId diversionId, DiversionSnapshot<T> diversionSnapshot) where T : class
        {
            _callRoutes[diversionId] = diversionSnapshot;
        }

        public bool Reset(DiversionId diversionId)
        {
            return _callRoutes.TryRemove(diversionId, out _);
        }
        
        public void ResetAll()
        {
            _callRoutes.Clear();
        }
    }
}