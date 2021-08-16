using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DivertR.Internal
{
    internal class RedirectRepository
    {
        private readonly ConcurrentDictionary<ViaId, object> _viaRedirects = new ConcurrentDictionary<ViaId, object>();

        public RedirectConfiguration<TTarget>? Get<TTarget>(ViaId viaId) where TTarget : class
        {
            return _viaRedirects.TryGetValue(viaId, out var existing)
                ? ((RedirectCollection<TTarget>) existing).RedirectConfiguration
                : null;
        }

        public RedirectConfiguration<TTarget> InsertRedirect<TTarget>(ViaId viaId, Redirect<TTarget> redirect) where TTarget : class
        {
            RedirectCollection<TTarget> Create(ViaId _)
            {
                return RedirectCollection<TTarget>.Empty.InsertRedirect(redirect);
            }

            RedirectCollection<TTarget> Update(ViaId _, object existing)
            {
                var redirectCollection = (RedirectCollection<TTarget>) existing;
                return redirectCollection.InsertRedirect(redirect);
            }
            
            var result = (RedirectCollection<TTarget>) _viaRedirects.AddOrUpdate(viaId, Create, Update);

            return result.RedirectConfiguration;
        }
        
        public RedirectConfiguration<TTarget> SetStrictMode<TTarget>(ViaId viaId) where TTarget : class
        {
            static RedirectCollection<TTarget> Create(ViaId _)
            {
                return RedirectCollection<TTarget>.Empty.SetStrictMode();
            }

            static RedirectCollection<TTarget> Update(ViaId _, object existing)
            {
                var redirectCollection = (RedirectCollection<TTarget>) existing;
                return redirectCollection.SetStrictMode();
            }
            
            var result = (RedirectCollection<TTarget>) _viaRedirects.AddOrUpdate(viaId, Create, Update);

            return result.RedirectConfiguration;
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
            private readonly ImmutableStack<InsertedRedirect<TTarget>> _redirectStack;
            
            public RedirectConfiguration<TTarget> RedirectConfiguration { get; }

            public static readonly RedirectCollection<TTarget> Empty =
                new RedirectCollection<TTarget>(0, ImmutableStack<InsertedRedirect<TTarget>>.Empty, false);

            private RedirectCollection(int insertSequence, ImmutableStack<InsertedRedirect<TTarget>> redirectStack, bool isStrictMode)
            {
                _insertSequence = insertSequence;
                _redirectStack = redirectStack;
                
                var redirectItems = _redirectStack
                    .OrderByDescending(x => x, RedirectComparer<TTarget>.Instance)
                    .Select(x => x.Redirect)
                    .ToArray();

                RedirectConfiguration = new RedirectConfiguration<TTarget>(redirectItems, isStrictMode);
            }

            public RedirectCollection<TTarget> InsertRedirect(Redirect<TTarget> redirect)
            {
                var inserted = new InsertedRedirect<TTarget>(redirect, _insertSequence + 1);
                return new RedirectCollection<TTarget>(inserted.InsertSequence, _redirectStack.Push(inserted), RedirectConfiguration.IsStrictMode);
            }
            
            public RedirectCollection<TTarget> SetStrictMode()
            {
                if (RedirectConfiguration.IsStrictMode)
                {
                    return this;
                }

                return new RedirectCollection<TTarget>(_insertSequence, _redirectStack, true);
            }
        }
        
        private class InsertedRedirect<TTarget> where TTarget : class
        {
            public Redirect<TTarget> Redirect { get; }
            public int InsertSequence { get; }

            public InsertedRedirect(Redirect<TTarget> redirect, int insertSequence)
            {
                Redirect = redirect;
                InsertSequence = insertSequence;
            }
        }

        private class RedirectComparer<TTarget> : IComparer<InsertedRedirect<TTarget>> where TTarget : class
        {
            public static readonly RedirectComparer<TTarget> Instance = new RedirectComparer<TTarget>();

            public int Compare(InsertedRedirect<TTarget> x, InsertedRedirect<TTarget> y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                var weightComparison = x.Redirect.OrderWeight.CompareTo(y.Redirect.OrderWeight);

                if (weightComparison != 0)
                {
                    return weightComparison;
                }

                return x.InsertSequence.CompareTo(y.InsertSequence);
            }
        }
    }
}
