using System.Collections.Generic;
using System.Linq;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class CompositeCallConstraint : ICallConstraint
    {
        private readonly IReadOnlyCollection<ICallConstraint> _callConstraints;

        public CompositeCallConstraint(IReadOnlyCollection<ICallConstraint> callConstraints)
        {
            _callConstraints = callConstraints;
        }

        public CompositeCallConstraint(IEnumerable<ICallConstraint> callConstraints)
        {
            _callConstraints = new List<ICallConstraint>(callConstraints);
        }

        public bool IsMatch(ICall call)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(call));
        }
    }
}