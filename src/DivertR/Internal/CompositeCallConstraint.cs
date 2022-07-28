using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class CompositeCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        private readonly ReadOnlyCollection<ICallConstraint<TTarget>> _callConstraints;
        
        public CompositeCallConstraint(IEnumerable<ICallConstraint<TTarget>> callConstraints)
        {
            _callConstraints = Array.AsReadOnly(callConstraints.ToArray());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo<TTarget> callInfo)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(callInfo));
        }
    }
    
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