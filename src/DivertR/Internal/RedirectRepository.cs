using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

        public IRedirect<TTarget>[] InsertRedirect<TTarget>(ViaId viaId, IRedirect<TTarget> redirect, int orderWeight = 0) where TTarget : class
        {
            RedirectCollection<TTarget> Create(ViaId _)
            {
                return RedirectCollection<TTarget>.Empty.InsertRedirect(redirect, orderWeight);
            }

            RedirectCollection<TTarget> Update(ViaId _, object existing)
            {
                var redirectCollection = (RedirectCollection<TTarget>) existing;
                return redirectCollection.InsertRedirect(redirect, orderWeight);
            }
            
            var result = (RedirectCollection<TTarget>) _viaRedirects.AddOrUpdate(viaId, Create, Update);

            return result.Redirects;
        }
        
        public bool Reset(ViaId viaId)
        {
            return _viaRedirects.TryRemove(viaId, out _);
        }
        
        public void ResetAll()
        {
            _viaRedirects.Clear();
        }

        private class RedirectCollection<TTarget> where TTarget : class
        {
            private readonly int _insertSequence;
            private readonly ImmutableStack<RedirectItem<TTarget>> _redirectStack;
            public IRedirect<TTarget>[] Redirects { get; }

            public static readonly RedirectCollection<TTarget> Empty =
                new RedirectCollection<TTarget>(0, ImmutableStack<RedirectItem<TTarget>>.Empty);

            private RedirectCollection(int insertSequence, ImmutableStack<RedirectItem<TTarget>> redirectStack)
            {
                _insertSequence = insertSequence;
                _redirectStack = redirectStack;
                
                Redirects = _redirectStack
                    .OrderByDescending(x => x, RedirectComparer<TTarget>.Instance)
                    .Select(x => x.Redirect)
                    .ToArray();
            }

            public RedirectCollection<TTarget> InsertRedirect(IRedirect<TTarget> redirect, int orderWeight)
            {
                var redirectItem = new RedirectItem<TTarget>(redirect, _insertSequence + 1, orderWeight);
                return new RedirectCollection<TTarget>(redirectItem.InsertSequence, _redirectStack.Push(redirectItem));
            }
        }
        
        private class RedirectItem<TTarget> where TTarget : class
        {
            public IRedirect<TTarget> Redirect { get; }
            public int InsertSequence { get; }
            public int OrderWeight { get; }

            public RedirectItem(IRedirect<TTarget> redirect, int insertSequence, int orderWeight)
            {
                Redirect = redirect;
                InsertSequence = insertSequence;
                OrderWeight = orderWeight;
            }
        }

        private class RedirectComparer<TTarget> : IComparer<RedirectItem<TTarget>> where TTarget : class
        {
            public static readonly RedirectComparer<TTarget> Instance = new RedirectComparer<TTarget>();

            public int Compare(RedirectItem<TTarget> x, RedirectItem<TTarget> y)
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

                return x.InsertSequence.CompareTo(y.InsertSequence);
            }
        }
    }
}
