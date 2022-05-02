using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectPlan<TTarget> : IRedirectPlan<TTarget> where TTarget : class
    {
        public static readonly RedirectPlan<TTarget> Empty = new RedirectPlan<TTarget>(ImmutableStack<IRedirect<TTarget>>.Empty, false);
        
        private readonly ImmutableStack<IRedirect<TTarget>> _redirectStack;

        private RedirectPlan(ImmutableStack<IRedirect<TTarget>> redirectStack, bool isStrictMode)
            : this(redirectStack, isStrictMode, OrderRedirects(redirectStack))
        {
        }
        
        private RedirectPlan(ImmutableStack<IRedirect<TTarget>> redirectStack, bool isStrictMode, IReadOnlyList<IRedirect<TTarget>> redirects)
        {
            _redirectStack = redirectStack;
            IsStrictMode = isStrictMode;
            Redirects = redirects;
        }

        public IReadOnlyList<IRedirect<TTarget>> Redirects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        private static IReadOnlyList<IRedirect<TTarget>> OrderRedirects(ImmutableStack<IRedirect<TTarget>> redirectStack)
        {
            return redirectStack
                .Select((r, i) => (r, i))
                .OrderByDescending(x => x, RedirectComparer<TTarget>.Instance)
                .Select(x => x.r)
                .ToArray();
        }
        
        internal RedirectPlan<TTarget> InsertRedirect(IRedirect<TTarget> redirect)
        {
            var mutatedStack = _redirectStack.Push(redirect);
            
            return new RedirectPlan<TTarget>(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan<TTarget> InsertRedirects(IEnumerable<IRedirect<TTarget>> redirects)
        {
            var mutatedStack = redirects.Aggregate(_redirectStack, (current, redirect) => current.Push(redirect));
            
            return new RedirectPlan<TTarget>(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan<TTarget> SetStrictMode(bool isStrict)
        {
            return new RedirectPlan<TTarget>(_redirectStack, isStrict, Redirects);
        }
        
        private class RedirectComparer<T> : IComparer<(IRedirect<T> redirect, int stackOrder)> where T : class
        {
            public static readonly RedirectComparer<T> Instance = new RedirectComparer<T>();
            
            public int Compare((IRedirect<T> redirect, int stackOrder) x, (IRedirect<T> redirect, int stackOrder) y)
            {
                var weightComparison = x.redirect.OrderWeight.CompareTo(y.redirect.OrderWeight);
                
                if (weightComparison != 0)
                {
                    return weightComparison;
                }

                return y.stackOrder.CompareTo(x.stackOrder);
            }
        }
    }
}