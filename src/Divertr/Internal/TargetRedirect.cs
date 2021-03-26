using DivertR.Core;

namespace DivertR.Internal
{
    internal class TargetRedirect<T> : IRedirect<T> where T : class
    {
        private readonly T _target;
        private readonly ICallConstraint<T> _callConstraint;

        public TargetRedirect(T target, ICallConstraint<T>? callConstraint = null)
        {
            _target = target;
            _callConstraint = callConstraint ?? TrueCallConstraint<T>.Instance;
        }

        public object? Call(CallInfo<T> callInfo)
        {
            if (_target == null)
            {
                throw new DiverterException("The redirect instance reference is null");
            }
            
            return callInfo.Invoke(_target);
        }

        public bool IsMatch(CallInfo<T> callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}