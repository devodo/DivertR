using System.Collections.Immutable;
using System.Linq;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class CompositeCallConstraint<T> : ICallConstraint<T> where T : class
    {
        private readonly ImmutableQueue<ICallConstraint<T>> _callConstraints;
        
        public static readonly CompositeCallConstraint<T> Empty = new CompositeCallConstraint<T>(ImmutableQueue<ICallConstraint<T>>.Empty);

        private CompositeCallConstraint(ImmutableQueue<ICallConstraint<T>> callConstraints)
        {
            _callConstraints = callConstraints;
        }

        public CompositeCallConstraint<T> AddCallConstraint(ICallConstraint<T> callConstraint)
        {
            return new CompositeCallConstraint<T>(_callConstraints.Enqueue(callConstraint));
        }
        
        public bool IsMatch(CallInfo<T> callInfo)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(callInfo));
        }
    }
}