namespace Divertr
{
    public interface IDiverter
    {
        object Proxy(object? origin = null);
    }
    
    public interface IDiverter<T> : IDiverter  where T : class
    {
        ICallContext<T> CallCtx { get; }
        T Proxy(T? original = null);
        IDiverter<T> Redirect(T target);
        IDiverter<T> AddRedirect(T target);
        IDiverter<T> Reset();
    }
}