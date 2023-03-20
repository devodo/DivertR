using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class CompositeCallConstraint : ICallConstraint
    {
        private readonly ReadOnlyCollection<ICallConstraint> _callConstraints;

        public CompositeCallConstraint(IEnumerable<ICallConstraint> callConstraints)
        {
            _callConstraints = Array.AsReadOnly(callConstraints.ToArray());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(callInfo));
        }
    }
}