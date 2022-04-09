using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectPlan : IRedirectPlan
    {
        public static readonly RedirectPlan Empty = new RedirectPlan(ImmutableStack<Redirect>.Empty, false);
        
        private readonly ImmutableStack<Redirect> _redirectStack;

        private RedirectPlan(ImmutableStack<Redirect> redirectStack, bool isStrictMode)
            : this(redirectStack, isStrictMode, OrderRedirects(redirectStack))
        {
        }
        
        private RedirectPlan(ImmutableStack<Redirect> redirectStack, bool isStrictMode, IReadOnlyList<Redirect> redirects)
        {
            _redirectStack = redirectStack;
            IsStrictMode = isStrictMode;
            Redirects = redirects;
        }

        public IReadOnlyList<Redirect> Redirects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        private static IReadOnlyList<Redirect> OrderRedirects(ImmutableStack<Redirect> redirectStack)
        {
            return redirectStack
                .Select((r, i) => (r, i))
                .OrderByDescending(x => x, RedirectComparer.Instance)
                .Select(x => x.r)
                .ToArray();
        }
        
        internal RedirectPlan InsertRedirect(Redirect redirect)
        {
            var mutatedStack = _redirectStack.Push(redirect);
            
            return new RedirectPlan(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan InsertRedirects(IEnumerable<Redirect> redirects)
        {
            var mutatedStack = redirects.Aggregate(_redirectStack, (current, redirect) => current.Push(redirect));
            
            return new RedirectPlan(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan SetStrictMode(bool isStrict)
        {
            return new RedirectPlan(_redirectStack, isStrict, Redirects);
        }
        
        private class RedirectComparer : IComparer<(Redirect redirect, int stackOrder)>
        {
            public static readonly RedirectComparer Instance = new RedirectComparer();
            
            public int Compare((Redirect redirect, int stackOrder) x, (Redirect redirect, int stackOrder) y)
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