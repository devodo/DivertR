namespace DivertR.Core
{
    public interface IVia
    {
        ViaId ViaId { get; }
        object ProxyObject(object? original = null);
    }
    
    public interface IVia<T> : IVia where T : class
    {
        IRelay<T> Relay { get; }
        T Proxy(T? original = null);
        IVia<T> RedirectTo(T target, object? state = null);
        IVia<T> AddRedirect(IRedirect<T> redirect);
        IVia<T> InsertRedirect(int index, T target, object? state = null);
        IVia<T> Reset();
    }
}