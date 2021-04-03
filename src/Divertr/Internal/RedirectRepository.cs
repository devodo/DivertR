using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectRepository
    {
        private readonly ConcurrentDictionary<ViaId, object> _viaRedirects = new ConcurrentDictionary<ViaId, object>();

        public IRedirect<TTarget>[] Get<TTarget>(ViaId viaId) where TTarget : class
        {
            return _viaRedirects.TryGetValue(viaId, out var existing) 
                ? ((RedirectCollection<TTarget>) existing).Redirects
                : RedirectCollection<TTarget>.Empty.Redirects;
        }

        public IRedirect<T>[] InsertRedirect<T>(ViaId viaId, IRedirect<T> redirect, int orderWeight = 0) where T : class
        {
            RedirectCollection<T> Create()
            {
                return RedirectCollection<T>.Empty.InsertRedirect(redirect, orderWeight);
            }

            RedirectCollection<T> Update(RedirectCollection<T> existing)
            {
                return existing.InsertRedirect(redirect, orderWeight);
            }

            return AddOrUpdate(viaId, Create, Update).Redirects;
        }
        
        public bool Reset(ViaId viaId)
        {
            return _viaRedirects.TryRemove(viaId, out _);
        }
        
        public void ResetAll()
        {
            _viaRedirects.Clear();
        }

        private RedirectCollection<T> AddOrUpdate<T>(
            ViaId viaId,
            Func<RedirectCollection<T>> addFactory,
            Func<RedirectCollection<T>, RedirectCollection<T>> updateFactory) where T : class
        {
            object Create(ViaId _)
            {
                return addFactory.Invoke();
            }

            object Update(ViaId _, object existing)
            {
                return updateFactory((RedirectCollection<T>) existing);
            }

            return (RedirectCollection<T>) _viaRedirects.AddOrUpdate(viaId, Create, Update);
        }

        private class RedirectCollection<T> where T : class
        {
            private readonly int _insertSequence;
            private readonly ImmutableStack<RedirectItem<T>> _redirectStack;
            public IRedirect<T>[] Redirects { get; }

            public static readonly RedirectCollection<T> Empty =
                new RedirectCollection<T>(0, ImmutableStack<RedirectItem<T>>.Empty);

            private RedirectCollection(int insertSequence, ImmutableStack<RedirectItem<T>> redirectStack)
            {
                _insertSequence = insertSequence;
                _redirectStack = redirectStack;
                
                Redirects = _redirectStack
                    .OrderBy(x => x, RedirectComparer<T>.Instance)
                    .Select(x => x.Redirect)
                    .ToArray();
            }

            public RedirectCollection<T> InsertRedirect(IRedirect<T> redirect, int orderWeight)
            {
                var redirectItem = new RedirectItem<T>(redirect, _insertSequence + 1, orderWeight);
                return new RedirectCollection<T>(redirectItem.InsertSequence, _redirectStack.Push(redirectItem));
            }
        }
        
        private class RedirectItem<T> where T : class
        {
            public IRedirect<T> Redirect { get; }
            public int InsertSequence { get; }
            public int OrderWeight { get; }

            public RedirectItem(IRedirect<T> redirect, int insertSequence, int orderWeight)
            {
                Redirect = redirect;
                InsertSequence = insertSequence;
                OrderWeight = orderWeight;
            }
        }

        private class RedirectComparer<T> : IComparer<RedirectItem<T>> where T : class
        {
            public static readonly RedirectComparer<T> Instance = new RedirectComparer<T>();

            public int Compare(RedirectItem<T> x, RedirectItem<T> y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                var weightComparison = x.OrderWeight.CompareTo(y.OrderWeight);

                if (weightComparison != 0)
                {
                    return weightComparison;
                }

                return y.InsertSequence.CompareTo(x.InsertSequence);
            }
        }
    }
}