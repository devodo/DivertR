namespace DivertR.Core
{
    public interface IRedirect<TTarget> where TTarget : class
    {
        object? Call(CallInfo<TTarget> callInfo);
        bool IsMatch(CallInfo<TTarget> callInfo);
    }
}