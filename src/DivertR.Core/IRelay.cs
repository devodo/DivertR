namespace DivertR.Core
{
    public interface IRelay<T> where T : class
    {
        T Next { get; }
        T Original { get; }
        IRedirect<T> Redirect { get; }
        CallInfo<T> CallInfo { get; }
        object? CallNext(CallInfo<T>? callInfo = null);
        object? CallOriginal(CallInfo<T>? callInfo = null);
    }
}