using System;
using System.Collections.Concurrent;

namespace Divertr.Internal
{
    internal class DiverterState
    {
        private readonly ConcurrentDictionary<DiverterId, object> _directors = new ConcurrentDictionary<DiverterId, object>();

        public CallRoute<T>? GetDirector<T>(DiverterId diverterId) where T : class
        {
            if (_directors.TryGetValue(diverterId, out var alteration))
            {
                return (CallRoute<T>)alteration;
            }

            return null;
        }

        public CallRoute<T> AddOrUpdateDirector<T>(DiverterId diverterId, Func<CallRoute<T>> addFactory, Func<CallRoute<T>, CallRoute<T>> updateFactory) where T : class
        {
            object Create(DiverterId _)
            {
                return addFactory.Invoke();
            }

            object Update(DiverterId _, object existingDiversion)
            {
                return updateFactory((CallRoute<T>) existingDiversion);
            }

            return (CallRoute<T>)_directors.AddOrUpdate(diverterId, Create, Update);
        }

        public void SetDirector<T>(DiverterId diverterId, CallRoute<T> callRoute) where T : class
        {
            _directors[diverterId] = callRoute;
        }

        public bool Reset(DiverterId diverterId)
        {
            return _directors.TryRemove(diverterId, out _);
        }
        
        public void Reset()
        {
            _directors.Clear();
        }
    }
}