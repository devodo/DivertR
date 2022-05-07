using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectPlan : IRedirectPlan
    {
        public static readonly RedirectPlan Empty = new RedirectPlan(ImmutableStack<IRedirect>.Empty, false);
        
        private readonly ImmutableStack<IRedirect> _redirectStack;

        private RedirectPlan(ImmutableStack<IRedirect> redirectStack, bool isStrictMode)
            : this(redirectStack, isStrictMode, OrderRedirects(redirectStack))
        {
        }
        
        private RedirectPlan(ImmutableStack<IRedirect> redirectStack, bool isStrictMode, IReadOnlyList<IRedirect> redirects)
        {
            _redirectStack = redirectStack;
            IsStrictMode = isStrictMode;
            Redirects = redirects;
        }

        public IReadOnlyList<IRedirect> Redirects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        private static IReadOnlyList<IRedirect> OrderRedirects(ImmutableStack<IRedirect> redirectStack)
        {
            return redirectStack
                .Select((r, i) => (r, i))
                .OrderByDescending(x => x, RedirectComparer.Instance)
                .Select(x => x.r)
                .ToArray();
        }
        
        internal RedirectPlan InsertRedirect(IRedirect redirect)
        {
            var mutatedStack = _redirectStack.Push(redirect);
            
            return new RedirectPlan(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan InsertRedirects(IEnumerable<IRedirect> redirects)
        {
            var mutatedStack = redirects.Aggregate(_redirectStack, (current, redirect) => current.Push(redirect));
            
            return new RedirectPlan(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan SetStrictMode(bool isStrict)
        {
            return new RedirectPlan(_redirectStack, isStrict, Redirects);
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