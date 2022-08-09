using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class CallConstraintWrapper<TTarget> : ICallConstraint<TTarget> where TTarget : class?
    {
        private readonly ICallConstraint _callConstraint;

        public CallConstraintWrapper(ICallConstraint callConstraint)
        {
            _callConstraint = callConstraint;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo<TTarget> callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}