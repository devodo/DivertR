using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectPlan<TRedirect> : IRedirectPlan<TRedirect> where TRedirect : IRedirect
    {
        public static readonly RedirectPlan<TRedirect> Empty = new RedirectPlan<TRedirect>(ImmutableStack<TRedirect>.Empty, false);
        
        private readonly ImmutableStack<TRedirect> _redirectStack;

        private RedirectPlan(ImmutableStack<TRedirect> redirectStack, bool isStrictMode)
            : this(redirectStack, isStrictMode, OrderRedirects(redirectStack))
        {
        }
        
        private RedirectPlan(ImmutableStack<TRedirect> redirectStack, bool isStrictMode, IReadOnlyList<TRedirect> redirects)
        {
            _redirectStack = redirectStack;
            IsStrictMode = isStrictMode;
            Redirects = redirects;
        }

        public IReadOnlyList<TRedirect> Redirects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        private static IReadOnlyList<TRedirect> OrderRedirects(ImmutableStack<TRedirect> redirectStack)
        {
            return redirectStack
                .Select((r, i) => (r, i))
                .OrderByDescending(x => x, RedirectComparer.Instance)
                .Select(x => x.r)
                .ToArray();
        }
        
        internal RedirectPlan<TRedirect> InsertRedirect(TRedirect redirect)
        {
            var mutatedStack = _redirectStack.Push(redirect);
            
            return new RedirectPlan<TRedirect>(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan<TRedirect> InsertRedirects(IEnumerable<TRedirect> redirects)
        {
            var mutatedStack = redirects.Aggregate(_redirectStack, (current, redirect) => current.Push(redirect));
            
            return new RedirectPlan<TRedirect>(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan<TRedirect> SetStrictMode(bool isStrict)
        {
            return new RedirectPlan<TRedirect>(_redirectStack, isStrict, Redirects);
        }
        
        private class RedirectComparer : IComparer<(IRedirect redirect, int stackOrder)>
        {
            public static readonly RedirectComparer Instance = new RedirectComparer();
            
            public int Compare((IRedirect redirect, int stackOrder) x, (IRedirect redirect, int stackOrder) y)
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