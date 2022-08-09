using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class CallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class?
    {
        private readonly Func<ICallInfo<TTarget>, bool> _constraintDelegate;

        public CallConstraint(Func<ICallInfo<TTarget>, bool> constraintDelegate)
        {
            _constraintDelegate = constraintDelegate ?? throw new ArgumentNullException(nameof(constraintDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo<TTarget> callInfo)
        {
            return _constraintDelegate.Invoke(callInfo);
        }
    }
    
    public class CallConstraint : ICallConstraint
    {
        private readonly Func<ICallInfo, bool> _constraintDelegate;

        public CallConstraint(Func<ICallInfo, bool> constraintDelegate)
        {
            _constraintDelegate = constraintDelegate ?? throw new ArgumentNullException(nameof(constraintDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            return _constraintDelegate.Invoke(callInfo);
        }
    }
}