using System;
using System.Collections.Concurrent;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ViaStateRepository
    {
        private readonly ConcurrentDictionary<ViaId, object> _viaStates = new ConcurrentDictionary<ViaId, object>();

        public ViaState<T>? Get<T>(ViaId viaId) where T : class
        {
            if (_viaStates.TryGetValue(viaId, out var alteration))
            {
                return (ViaState<T>)alteration;
            }

            return null;
        }
        
        public void Set<T>(ViaId viaId, ViaState<T> viaState) where T : class
        {
            _viaStates[viaId] = viaState;
        }

        public ViaState<T> AddOrUpdate<T>(ViaId viaId, Func<ViaState<T>> addFactory, Func<ViaState<T>, ViaState<T>> updateFactory) where T : class
        {
            object Create(ViaId _)
            {
                return addFactory.Invoke();
            }

            object Update(ViaId _, object existing)
            {
                return updateFactory((ViaState<T>) existing);
            }

            return (ViaState<T>)_viaStates.AddOrUpdate(viaId, Create, Update);
        }
        
        public bool Reset(ViaId viaId)
        {
            return _viaStates.TryRemove(viaId, out _);
        }
        
        public void ResetAll()
        {
            _viaStates.Clear();
        }
    }
}