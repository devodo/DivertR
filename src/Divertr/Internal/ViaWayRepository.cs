using System;
using System.Collections.Concurrent;

namespace DivertR.Internal
{
    internal class ViaWayRepository
    {
        private readonly ConcurrentDictionary<ViaId, object> _viaWays = new ConcurrentDictionary<ViaId, object>();

        public ViaWay<T>? Get<T>(ViaId viaId) where T : class
        {
            if (_viaWays.TryGetValue(viaId, out var alteration))
            {
                return (ViaWay<T>)alteration;
            }

            return null;
        }
        
        public void Set<T>(ViaId viaId, ViaWay<T> viaWay) where T : class
        {
            _viaWays[viaId] = viaWay;
        }

        public ViaWay<T> AddOrUpdate<T>(ViaId viaId, Func<ViaWay<T>> addFactory, Func<ViaWay<T>, ViaWay<T>> updateFactory) where T : class
        {
            object Create(ViaId _)
            {
                return addFactory.Invoke();
            }

            object Update(ViaId _, object existing)
            {
                return updateFactory((ViaWay<T>) existing);
            }

            return (ViaWay<T>)_viaWays.AddOrUpdate(viaId, Create, Update);
        }
        
        public bool Reset(ViaId viaId)
        {
            return _viaWays.TryRemove(viaId, out _);
        }
        
        public void ResetAll()
        {
            _viaWays.Clear();
        }
    }
}