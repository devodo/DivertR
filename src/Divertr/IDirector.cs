namespace Divertr
{
    public interface IDirector
    {
        object Proxy(object? origin = null);
    }
    
    public interface IDirector<T> : IDirector  where T : class
    {
        ICallContext<T> CallCtx { get; }
        T Proxy(T? original = null);
        IDirector<T> Redirect(T target);
        IDirector<T> AddRedirect(T target);
        IDirector<T> Reset();
    }
}