using DivertR.Internal;

namespace DivertR
{
    public static class TargetExtensions
    {
        public static ICallConstraint<TTarget> Of<TTarget>(this ICallConstraint callConstraint) where TTarget : class?
        {
            return new CallConstraintWrapper<TTarget>(callConstraint);
        }

        public static ICallHandler<TTarget> Of<TTarget>(this ICallHandler callHandler) where TTarget : class?
        {
            return new CallHandlerWrapper<TTarget>(callHandler);
        }
    }
}