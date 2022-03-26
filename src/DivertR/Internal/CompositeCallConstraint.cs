using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class CompositeCallConstraint : ICallConstraint
    {
        private readonly ImmutableArray<ICallConstraint> _callConstraints;
        
        public static readonly CompositeCallConstraint Empty = new CompositeCallConstraint(ImmutableArray<ICallConstraint>.Empty);

        private CompositeCallConstraint(ImmutableArray<ICallConstraint> callConstraints)
        {
            _callConstraints = callConstraints;
        }

        public CompositeCallConstraint AddCallConstraint(ICallConstraint callConstraint)
        {
            return new CompositeCallConstraint(_callConstraints.Add(callConstraint));
        }
        
        public CompositeCallConstraint AddCallConstraints(IEnumerable<ICallConstraint> callConstraints)
        {
            return new CompositeCallConstraint(_callConstraints.AddRange(callConstraints));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo callInfo)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(callInfo));
        }
    }
}