using System.Runtime.CompilerServices;

namespace DivertR
{
    internal class WrappedRedirect<TTarget> : IRedirect<TTarget> where TTarget : class
    {
        public WrappedRedirect(IRedirect redirect)
        {
            OrderWeight = redirect.OrderWeight;
            DisableSatisfyStrict = redirect.DisableSatisfyStrict;
            CallConstraint = new WrappedCallConstraint<TTarget>(redirect.CallConstraint);
            CallHandler = new WrappedCallHandler<TTarget>(redirect.CallHandler);
        }

        public int OrderWeight { get; }

        public bool DisableSatisfyStrict
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public ICallConstraint<TTarget> CallConstraint
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public ICallHandler<TTarget> CallHandler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}