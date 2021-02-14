namespace Divertr
{
    public interface IDiversion
    {
        object Proxy(object? root = null);
    }
    
    public interface IDiversion<T> : IDiversion  where T : class
    {
        ICallContext<T> CallCtx { get; }
        T Proxy(T? root = null);
        IDiversion<T> Redirect(T target);
        IDiversion<T> AddRedirect(T target);
        IDiversion<T> Reset();
    }
}