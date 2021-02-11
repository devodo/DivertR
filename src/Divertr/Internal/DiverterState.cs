using System;
using System.Collections.Concurrent;

namespace Divertr.Internal
{
    internal class DiverterState
    {
        private readonly ConcurrentDictionary<DiverterId, object> _directors = new ConcurrentDictionary<DiverterId, object>();

        public Director<T>? GetDirector<T>(DiverterId diverterId) where T : class
        {
            if (_directors.TryGetValue(diverterId, out var alteration))
            {
                return (Director<T>)alteration;
            }

            return null;
        }

        public Director<T> AddOrUpdateDirector<T>(DiverterId diverterId, Func<Director<T>> addFactory, Func<Director<T>, Director<T>> updateFactory) where T : class
        {
            object Create(DiverterId _)
            {
                return addFactory.Invoke();
            }

            object Update(DiverterId _, object existingDiversion)
            {
                return updateFactory((Director<T>) existingDiversion);
            }

            return (Director<T>)_directors.AddOrUpdate(diverterId, Create, Update);
        }

        public void SetDirector<T>(DiverterId diverterId, Director<T> director) where T : class
        {
            _directors[diverterId] = director;
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