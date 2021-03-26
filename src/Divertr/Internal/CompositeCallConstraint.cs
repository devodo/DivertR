using System.Collections.Generic;
using System.Linq;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class CompositeCallConstraint<T> : ICallConstraint<T> where T : class
    {
        private readonly IReadOnlyCollection<ICallConstraint<T>> _callConstraints;

        public CompositeCallConstraint(IReadOnlyCollection<ICallConstraint<T>> callConstraints)
        {
            _callConstraints = callConstraints;
        }

        public CompositeCallConstraint(IEnumerable<ICallConstraint<T>> callConstraints)
        {
            _callConstraints = new List<ICallConstraint<T>>(callConstraints);
        }

        public bool IsMatch(CallInfo<T> callInfo)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(callInfo));
        }
    }
}