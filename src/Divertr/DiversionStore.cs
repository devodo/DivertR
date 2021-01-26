using System;
using System.Collections.Concurrent;

namespace Divertr
{
    internal class DiversionStore
    {
        private readonly ConcurrentDictionary<DiversionId, object> _diversions = new ConcurrentDictionary<DiversionId, object>();

        public Diversion<T> GetDiversion<T>(DiversionId diversionId) where T : class
        {
            if (_diversions.TryGetValue(diversionId, out var alteration))
            {
                return (Diversion<T>)alteration;
            }

            return null;
        }

        public Diversion<T> AddOrUpdateDiversion<T>(DiversionId diversionId, Func<Diversion<T>> addFactory, Func<Diversion<T>, Diversion<T>> updateFactory) where T : class
        {
            object Create(DiversionId _)
            {
                return addFactory.Invoke();
            }

            object Update(DiversionId _, object existingDiversion)
            {
                return updateFactory((Diversion<T>) existingDiversion);
            }

            return (Diversion<T>)_diversions.AddOrUpdate(diversionId, Create, Update);
        }

        public void SetDiversion<T>(DiversionId diversionId, Diversion<T> diversion) where T : class
        {
            _diversions[diversionId] = diversion;
        }

        public bool Reset(DiversionId diversionId)
        {
            return _diversions.TryRemove(diversionId, out _);
        }
        
        public void Reset()
        {
            _diversions.Clear();
        }
    }
}