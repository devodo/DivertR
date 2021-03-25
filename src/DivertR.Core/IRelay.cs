namespace DivertR.Core
{
    public interface IRelay<out T> where T : class
    {
        T Next { get; }
        T Original { get; }
        T? OriginalInstance { get; }
        IRedirect<T> Redirect { get; }
        T ProxyInstance { get; }
        CallInfo CallInfo { get; }
        object? CallNext(CallInfo callInfo);
        object? CallOriginal(CallInfo callInfo);
    }
}