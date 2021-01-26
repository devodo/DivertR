namespace Divertr
{
    public interface IDiverter<T> where T : class
    {
        ICallContext<T> CallContext { get; }
        T Proxy(T origin = null);
        IDiverter<T> Redirect(T redirect);
        IDiverter<T> AddRedirect(T substitute);
        IDiverter<T> Reset();
    }
}