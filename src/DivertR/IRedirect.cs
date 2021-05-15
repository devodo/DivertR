using DivertR.Core;

namespace DivertR
{
    public interface IRedirect<TTarget> where TTarget : class
    {
        public ICallConstraint<TTarget> CallConstraint { get; }
        object? Call(CallInfo<TTarget> callInfo);
    }
}