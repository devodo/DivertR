using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectPlan : IRedirectPlan
    {
        public static readonly RedirectPlan Empty = new(ImmutableStack<IViaContainer>.Empty, false);
        
        private readonly ImmutableStack<IViaContainer> _viaStack;

        private RedirectPlan(ImmutableStack<IViaContainer> viaStack, bool isStrictMode)
            : this(viaStack, isStrictMode, OrderVias(viaStack))
        {
        }
        
        private RedirectPlan(ImmutableStack<IViaContainer> viaStack, bool isStrictMode, IReadOnlyList<IViaContainer> vias)
        {
            _viaStack = viaStack;
            IsStrictMode = isStrictMode;
            Vias = vias;
        }

        public IReadOnlyList<IViaContainer> Vias
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        private static IReadOnlyList<IViaContainer> OrderVias(ImmutableStack<IViaContainer> viaStack)
        {
            return viaStack
                .Select((via, i) => (via, i))
                .OrderByDescending(x => x, ViaComparer.Instance)
                .Select(x => x.via)
                .ToArray();
        }
        
        internal RedirectPlan InsertVia(IViaContainer via)
        {
            var mutatedStack = _viaStack.Push(via);
            
            return new RedirectPlan(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan SetStrictMode(bool isStrict)
        {
            return new RedirectPlan(_viaStack, isStrict, Vias);
        }
        
        private class ViaComparer : IComparer<(IViaContainer via, int stackOrder)>
        {
            public static readonly ViaComparer Instance = new();
            
            public int Compare((IViaContainer via, int stackOrder) x, (IViaContainer via, int stackOrder) y)
            {
                var weightComparison = x.via.Options.OrderWeight.CompareTo(y.via.Options.OrderWeight);
                
                if (weightComparison != 0)
                {
                    return weightComparison;
                }

                return y.stackOrder.CompareTo(x.stackOrder);
            }
        }
    }
}