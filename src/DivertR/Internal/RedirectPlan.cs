using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectPlan : IRedirectPlan
    {
        public static readonly RedirectPlan Empty = new(ImmutableStack<IConfiguredVia>.Empty, false);
        
        private readonly ImmutableStack<IConfiguredVia> _viaStack;

        private RedirectPlan(ImmutableStack<IConfiguredVia> viaStack, bool isStrictMode)
            : this(viaStack, isStrictMode, OrderVias(viaStack))
        {
        }
        
        private RedirectPlan(ImmutableStack<IConfiguredVia> viaStack, bool isStrictMode, IReadOnlyList<IConfiguredVia> vias)
        {
            _viaStack = viaStack;
            IsStrictMode = isStrictMode;
            Vias = vias;
        }

        public IReadOnlyList<IConfiguredVia> Vias
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        private static IReadOnlyList<IConfiguredVia> OrderVias(ImmutableStack<IConfiguredVia> viaStack)
        {
            return viaStack
                .Select((via, i) => (via, i))
                .OrderByDescending(x => x, ViaComparer.Instance)
                .Select(x => x.via)
                .ToArray();
        }
        
        internal RedirectPlan InsertVia(IConfiguredVia configuredVia)
        {
            var mutatedStack = _viaStack.Push(configuredVia);
            
            return new RedirectPlan(mutatedStack, IsStrictMode);
        }
        
        internal RedirectPlan SetStrictMode(bool isStrict)
        {
            return new RedirectPlan(_viaStack, isStrict, Vias);
        }
        
        private class ViaComparer : IComparer<(IConfiguredVia via, int stackOrder)>
        {
            public static readonly ViaComparer Instance = new();
            
            public int Compare((IConfiguredVia via, int stackOrder) x, (IConfiguredVia via, int stackOrder) y)
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