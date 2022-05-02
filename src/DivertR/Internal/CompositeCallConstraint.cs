using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class CompositeCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        private readonly ImmutableArray<ICallConstraint<TTarget>> _callConstraints;
        
        public static readonly CompositeCallConstraint<TTarget> Empty = new CompositeCallConstraint<TTarget>(ImmutableArray<ICallConstraint<TTarget>>.Empty);

        private CompositeCallConstraint(ImmutableArray<ICallConstraint<TTarget>> callConstraints)
        {
            _callConstraints = callConstraints;
        }

        public CompositeCallConstraint<TTarget> AddCallConstraint(ICallConstraint<TTarget> callConstraint)
        {
            return new CompositeCallConstraint<TTarget>(_callConstraints.Add(callConstraint));
        }
        
        public CompositeCallConstraint<TTarget> AddCallConstraints(IEnumerable<ICallConstraint<TTarget>> callConstraints)
        {
            return new CompositeCallConstraint<TTarget>(_callConstraints.AddRange(callConstraints));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo<TTarget> callInfo)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(callInfo));
        }
    }
}