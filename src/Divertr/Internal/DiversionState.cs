using System;
using System.Collections.Concurrent;

namespace Divertr.Internal
{
    internal class DiversionState
    {
        private readonly ConcurrentDictionary<DiverterId, object> _diversions = new ConcurrentDictionary<DiverterId, object>();

        public Diversion<T>? GetDiversion<T>(DiverterId diverterId) where T : class
        {
            if (_diversions.TryGetValue(diverterId, out var alteration))
            {
                return (Diversion<T>)alteration;
            }

            return null;
        }

        public Diversion<T> AddOrUpdateDiversion<T>(DiverterId diverterId, Func<Diversion<T>> addFactory, Func<Diversion<T>, Diversion<T>> updateFactory) where T : class
        {
            object Create(DiverterId _)
            {
                return addFactory.Invoke();
            }

            object Update(DiverterId _, object existingDiversion)
            {
                return updateFactory((Diversion<T>) existingDiversion);
            }

            return (Diversion<T>)_diversions.AddOrUpdate(diverterId, Create, Update);
        }

        public void SetDiversion<T>(DiverterId diverterId, Diversion<T> diversion) where T : class
        {
            _diversions[diverterId] = diversion;
        }

        public bool Reset(DiverterId diverterId)
        {
            return _diversions.TryRemove(diverterId, out _);
        }
        
        public void Reset()
        {
            _diversions.Clear();
        }
    }
}