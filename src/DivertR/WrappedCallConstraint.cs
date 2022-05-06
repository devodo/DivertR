using System.Runtime.CompilerServices;

namespace DivertR
{
    internal class WrappedCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        private readonly ICallConstraint _innerConstraint;

        public WrappedCallConstraint(ICallConstraint innerConstraint)
        {
            _innerConstraint = innerConstraint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo<TTarget> callInfo)
        {
            return _innerConstraint.IsMatch(callInfo);
        }
    }
}