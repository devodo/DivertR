using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class TargetRedirect<TTarget> : IRedirect<TTarget> where TTarget : class
    {
        private readonly TTarget _target;

        public TargetRedirect(TTarget target, ICallConstraint<TTarget>? callConstraint = null)
        {
            _target = target;
            CallConstraint = callConstraint ?? TrueCallConstraint<TTarget>.Instance;
        }

        public ICallConstraint<TTarget> CallConstraint { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            if (_target == null)
            {
                throw new DiverterException("The redirect instance reference is null");
            }
            
            return callInfo.Invoke(_target);
        }
    }
}