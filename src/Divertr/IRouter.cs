namespace Divertr
{
    public interface IRouter
    {
        RouterId RouterId { get; }
        object Proxy(object? original = null);
    }
    
    public interface IRouter<T> : IRouter  where T : class
    {
        ICallRelay<T> Relay { get; }
        T Proxy(T? original = null);
        IRouter<T> Redirect(T target);
        IRouter<T> AddRedirect(T target);
        IRouter<T> Reset();
    }
}