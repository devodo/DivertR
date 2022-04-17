namespace DivertR
{
    public interface IProxyCall<TTarget> where TTarget : class
    {
        object? Call(CallInfo<TTarget> callInfo);
    }
}