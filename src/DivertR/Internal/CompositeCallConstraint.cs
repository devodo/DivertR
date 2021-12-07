using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class CompositeCallConstraint<T> : ICallConstraint<T> where T : class
    {
        private readonly ImmutableArray<ICallConstraint<T>> _callConstraints;
        
        public static readonly CompositeCallConstraint<T> Empty = new CompositeCallConstraint<T>(ImmutableArray<ICallConstraint<T>>.Empty);

        private CompositeCallConstraint(ImmutableArray<ICallConstraint<T>> callConstraints)
        {
            _callConstraints = callConstraints;
        }

        public CompositeCallConstraint<T> AddCallConstraint(ICallConstraint<T> callConstraint)
        {
            return new CompositeCallConstraint<T>(_callConstraints.Add(callConstraint));
        }
        
        public CompositeCallConstraint<T> AddCallConstraints(IEnumerable<ICallConstraint<T>> callConstraints)
        {
            return new CompositeCallConstraint<T>(_callConstraints.AddRange(callConstraints));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo<T> callInfo)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(callInfo));
        }
    }
}