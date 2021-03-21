namespace DivertR.Core
{
    public interface IRelay<out T> where T : class
    {
        T Next { get; }
        T Original { get; }
        T? OriginalInstance { get; }
        IRedirect<T> Redirect { get; }
        //object? State { get; }
        object? CallNext(ICall call);
    }
}