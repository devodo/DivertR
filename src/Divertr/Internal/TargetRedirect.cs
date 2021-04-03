using DivertR.Core;

namespace DivertR.Internal
{
    internal class TargetRedirect<TTarget> : IRedirect<TTarget> where TTarget : class
    {
        private readonly TTarget _target;
        private readonly ICallConstraint<TTarget> _callConstraint;

        public TargetRedirect(TTarget target, ICallConstraint<TTarget>? callConstraint = null)
        {
            _target = target;
            _callConstraint = callConstraint ?? TrueCallConstraint<TTarget>.Instance;
        }

        public object? Call(CallInfo<TTarget> callInfo)
        {
            if (_target == null)
            {
                throw new DiverterException("The redirect instance reference is null");
            }
            
            return callInfo.Invoke(_target);
        }

        public bool IsMatch(CallInfo<TTarget> callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}