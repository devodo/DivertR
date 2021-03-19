using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class TargetRedirect<T> : IRedirect<T> where T : class
    {
        private readonly T _target;
        private readonly ICallConstraint _callConstraint;
        public object? State { get; }

        public TargetRedirect(T target, object? state = null, ICallConstraint? callCondition = null)
        {
            _callConstraint = callCondition ?? TrueCallConstraint.Instance;
            _target = target;
            State = state;
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
            return _callConstraint.IsMatch(call);
        }
    }
}
