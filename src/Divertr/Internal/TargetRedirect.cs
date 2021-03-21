using System.Collections.Generic;
using System.Linq;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class TargetRedirect<T> : IRedirect<T> where T : class
    {
        private readonly T _target;
        private readonly IEnumerable<ICallConstraint> _callConstraints;
        public object? State { get; }

        public TargetRedirect(T target, object? state = null, IEnumerable<ICallConstraint>? callConstraints = null)
        {
            _target = target;
            State = state;
            _callConstraints = callConstraints ?? Enumerable.Empty<ICallConstraint>();
        }

        public object? Invoke(ICall call)
        {
            if (_target == null)
            {
                throw new DiverterException("The redirect instance reference is null");
            }
            
            return call.Method.ToDelegate(typeof(T)).Invoke(_target, call.Arguments);
        }

        public bool IsMatch(ICall call)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(call));
        }
    }
}
