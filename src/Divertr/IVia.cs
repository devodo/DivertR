namespace DivertR
{
    public interface IVia
    {
        ViaId ViaId { get; }
        object ProxyObject(object? original = null);
    }
    
    public interface IVia<T> : IVia  where T : class
    {
        IRelay<T> Relay { get; }
        T Proxy(T? original = null);
        IVia<T> Redirect(T target, object? state = null);
        IVia<T> AddRedirect(T target, object? state = null);
        IVia<T> InsertRedirect(int index, T target, object? state = null);
        IVia<T> Reset();
    }
}