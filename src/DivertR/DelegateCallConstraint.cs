using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class DelegateCallConstraint<TTarget> : CallConstraint<TTarget> where TTarget : class?
    {
        private readonly Func<ICallInfo<TTarget>, bool> _constraintDelegate;

        public DelegateCallConstraint(Func<ICallInfo<TTarget>, bool> constraintDelegate)
        {
            _constraintDelegate = constraintDelegate ?? throw new ArgumentNullException(nameof(constraintDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool IsMatch(ICallInfo<TTarget> callInfo)
        {
            return _constraintDelegate.Invoke(callInfo);
        }
    }
    
    public class DelegateCallConstraint : ICallConstraint
    {
        private readonly Func<ICallInfo, bool> _constraintDelegate;

        public DelegateCallConstraint(Func<ICallInfo, bool> constraintDelegate)
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