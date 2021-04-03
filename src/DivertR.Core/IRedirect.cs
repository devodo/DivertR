namespace DivertR.Core
{
    public interface IRedirect<TTarget> where TTarget : class
    {
        public ICallConstraint<TTarget> CallConstraint { get; }
        object? Call(CallInfo<TTarget> callInfo);
    }
}