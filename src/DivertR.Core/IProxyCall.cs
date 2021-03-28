namespace DivertR.Core
{
    public interface IProxyCall<T> where T : class
    {
        object? Call(CallInfo<T> callInfo);
    }
}