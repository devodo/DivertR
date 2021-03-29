using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectRepository
    {
        private readonly ConcurrentDictionary<ViaId, object> _viaRedirects = new ConcurrentDictionary<ViaId, object>();

        public ImmutableArray<IRedirect<T>> Get<T>(ViaId viaId) where T : class
        {
            if (_viaRedirects.TryGetValue(viaId, out var redirects))
            {
                return (ImmutableArray<IRedirect<T>>) redirects;
            }

            return ImmutableArray<IRedirect<T>>.Empty;
        }
        
        public ImmutableArray<IRedirect<T>> AddRedirect<T>(ViaId viaId, IRedirect<T> redirect) where T : class
        {
            ImmutableArray<IRedirect<T>> Create()
            {
                return ImmutableArray.Create(redirect);
            }

            ImmutableArray<IRedirect<T>> Update(ImmutableArray<IRedirect<T>> existing)
            {
                return existing.Add(redirect);
            }

            return AddOrUpdate(viaId, Create, Update);
        }
        
        public ImmutableArray<IRedirect<T>> InsertRedirect<T>(ViaId viaId, int index, IRedirect<T> redirect) where T : class
        {
            ImmutableArray<IRedirect<T>> Create()
            {
                if (index != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Insert index must be 0 as there are no existing redirects");
                }

                return ImmutableArray<IRedirect<T>>.Empty.Insert(0, redirect);
            }

            ImmutableArray<IRedirect<T>> Update(ImmutableArray<IRedirect<T>> existing)
            {
                if (index < 0 || index > existing.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index not within the bounds of the existing redirects");
                }

                return existing.Insert(index, redirect);
            }

            return AddOrUpdate(viaId, Create, Update);
        }

        public ImmutableArray<IRedirect<T>> RemoveRedirect<T>(ViaId viaId, IRedirect<T> redirect) where T : class
        {
            ImmutableArray<IRedirect<T>> Create()
            {
                return ImmutableArray<IRedirect<T>>.Empty;
            }

            ImmutableArray<IRedirect<T>> Update(ImmutableArray<IRedirect<T>> existing)
            {
                return existing.Remove(redirect);
            }

            return AddOrUpdate(viaId, Create, Update);
        }

        public ImmutableArray<IRedirect<T>> RemoveRedirectAt<T>(ViaId viaId, int index) where T : class
        {
            ImmutableArray<IRedirect<T>> Create()
            {
                throw new ArgumentOutOfRangeException(nameof(index), "No index valid as there are no existing redirects");
            }

            ImmutableArray<IRedirect<T>> Update(ImmutableArray<IRedirect<T>> existing)
            {
                if (index < 0 || index > existing.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index not within the bounds of the existing redirects");
                }

                return existing.RemoveAt(index);
            }

            return AddOrUpdate(viaId, Create, Update);
        }

        public bool Reset(ViaId viaId)
        {
            return _viaRedirects.TryRemove(viaId, out _);
        }
        
        public void ResetAll()
        {
            _viaRedirects.Clear();
        }

        private ImmutableArray<IRedirect<T>> AddOrUpdate<T>(
            ViaId viaId,
            Func<ImmutableArray<IRedirect<T>>> addFactory,
            Func<ImmutableArray<IRedirect<T>>, ImmutableArray<IRedirect<T>>> updateFactory) where T : class
        {
            object Create(ViaId _)
            {
                return addFactory.Invoke();
            }

            object Update(ViaId _, object existing)
            {
                return updateFactory((ImmutableArray<IRedirect<T>>) existing);
            }

            return (ImmutableArray<IRedirect<T>>) _viaRedirects.AddOrUpdate(viaId, Create, Update);
        }
    }
}