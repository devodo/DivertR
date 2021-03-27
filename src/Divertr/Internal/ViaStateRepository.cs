using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
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
                return (ViaState<T>) alteration;
            }

            return null;
        }
        
        public ViaState<T> AddRedirect<T>(ViaId viaId, IRedirect<T> redirect) where T : class
        {
            ViaState<T> Create()
            {
                return new ViaState<T>(ImmutableArray.Create(redirect));
            }

            ViaState<T> Update(ViaState<T> existing)
            {
                var redirects = existing.Redirects.Add(redirect);
                return new ViaState<T>(redirects);
            }

            return AddOrUpdate(viaId, Create, Update);
        }
        
        public ViaState<T> InsertRedirect<T>(ViaId viaId, int index, IRedirect<T> redirect) where T : class
        {
            ViaState<T> Create()
            {
                if (index != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0 if there are no existing redirects");
                }
                
                return new ViaState<T>(ImmutableArray.Create(redirect));
            }

            ViaState<T> Update(ViaState<T> existing)
            {
                if (index < 0 || index > existing.Redirects.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the existing redirects");
                }

                var redirects = existing.Redirects.Insert(index, redirect);
                return new ViaState<T>(redirects);
            }

            return AddOrUpdate(viaId, Create, Update);
        }
        
        public bool Reset(ViaId viaId)
        {
            return _viaStates.TryRemove(viaId, out _);
        }
        
        public void ResetAll()
        {
            _viaStates.Clear();
        }

        private ViaState<T> AddOrUpdate<T>(ViaId viaId, Func<ViaState<T>> addFactory, Func<ViaState<T>, ViaState<T>> updateFactory) where T : class
        {
            object Create(ViaId _)
            {
                return addFactory.Invoke();
            }

            object Update(ViaId _, object existing)
            {
                return updateFactory((ViaState<T>) existing);
            }

            return (ViaState<T>) _viaStates.AddOrUpdate(viaId, Create, Update);
        }
    }
}