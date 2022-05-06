namespace DivertR
{
    public interface IProxyCall<TTarget> where TTarget : class
    {
        object? Call(ICallInfo<TTarget> callInfo);
    }
}