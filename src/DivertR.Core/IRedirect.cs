namespace DivertR.Core
{
    public interface IRedirect<T> where T : class
    {
        object? Call(CallInfo<T> callInfo);
        bool IsMatch(CallInfo<T> callInfo);
    }
}