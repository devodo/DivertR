using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectPlan<TTarget> : IRedirectPlan<TTarget> where TTarget : class
    {
        public static readonly RedirectPlan<TTarget> Empty = new RedirectPlan<TTarget>(ImmutableStack<Redirect<TTarget>>.Empty, false);
        
        private readonly ImmutableStack<Redirect<TTarget>> _redirectStack;

        private RedirectPlan(ImmutableStack<Redirect<TTarget>> redirectStack, bool isStrictMode)
            : this(redirectStack, isStrictMode, OrderRedirects(redirectStack))
        {
        }
        
        private RedirectPlan(ImmutableStack<Redirect<TTarget>> redirectStack, bool isStrictMode, IReadOnlyList<Redirect<TTarget>> redirects)
        {
            _redirectStack = redirectStack;
            IsStrictMode = isStrictMode;
            Redirects = redirects;
        }

        public IReadOnlyList<Redirect<TTarget>> Redirects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        private static IReadOnlyList<Redirect<TTarget>> OrderRedirects(ImmutableStack<Redirect<TTarget>> redirectStack)
        {
            return redirectStack
                .Select((r, i) => (r, i))
                .OrderByDescending(x => x, RedirectComparer.Instance)
                .Select(x => x.r)
                .ToArray();
        }
        
        internal RedirectPlan<TTarget> InsertRedirect(Redirect<TTarget> redirect)
        {
            var mutatedStack = _redirectStack.Push(redirect);
            
            return new RedirectPlan<TTarget>(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan<TTarget> SetStrictMode(bool isStrict)
        {
            return new RedirectPlan<TTarget>(_redirectStack, isStrict, Redirects);
        }
        
        private class RedirectComparer : IComparer<(Redirect<TTarget> Redirect, int StackOrder)>
        {
            public static readonly RedirectComparer Instance = new RedirectComparer();
            
            public int Compare((Redirect<TTarget> Redirect, int StackOrder) x, (Redirect<TTarget> Redirect, int StackOrder) y)
            {
                var weightComparison = x.Redirect.OrderWeight.CompareTo(y.Redirect.OrderWeight);
                
                if (weightComparison != 0)
                {
                    return weightComparison;
                }

                return y.StackOrder.CompareTo(x.StackOrder);
            }
        }
    }
}