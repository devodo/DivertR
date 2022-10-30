using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectPlan : IRedirectPlan
    {
        public static readonly RedirectPlan Empty = new(ImmutableStack<IRedirectContainer>.Empty, false);
        
        private readonly ImmutableStack<IRedirectContainer> _redirectStack;

        private RedirectPlan(ImmutableStack<IRedirectContainer> redirectStack, bool isStrictMode)
            : this(redirectStack, isStrictMode, OrderRedirects(redirectStack))
        {
        }
        
        private RedirectPlan(ImmutableStack<IRedirectContainer> redirectStack, bool isStrictMode, IReadOnlyList<IRedirectContainer> redirects)
        {
            _redirectStack = redirectStack;
            IsStrictMode = isStrictMode;
            Stack = redirects;
        }

        public IReadOnlyList<IRedirectContainer> Stack
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        private static IReadOnlyList<IRedirectContainer> OrderRedirects(ImmutableStack<IRedirectContainer> redirectStack)
        {
            return redirectStack
                .Select((redirect, i) => (redirect, i))
                .OrderByDescending(x => x, RedirectComparer.Instance)
                .Select(x => x.redirect)
                .ToArray();
        }
        
        internal RedirectPlan InsertRedirect(IRedirectContainer redirect)
        {
            var mutatedStack = _redirectStack.Push(redirect);
            
            return new RedirectPlan(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan SetStrictMode(bool isStrict)
        {
            return new RedirectPlan(_redirectStack, isStrict, Stack);
        }
        
        private class RedirectComparer : IComparer<(IRedirectContainer redirect, int stackOrder)>
        {
            public static readonly RedirectComparer Instance = new();
            
            public int Compare((IRedirectContainer redirect, int stackOrder) x, (IRedirectContainer redirect, int stackOrder) y)
            {
                var weightComparison = x.redirect.Options.OrderWeight.CompareTo(y.redirect.Options.OrderWeight);
                
                if (weightComparison != 0)
                {
                    return weightComparison;
                }

                return y.stackOrder.CompareTo(x.stackOrder);
            }
        }
    }
}