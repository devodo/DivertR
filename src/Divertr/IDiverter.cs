using Divertr.Internal;

namespace Divertr
{
    public interface IDiverter
    {
        DiverterId DiverterId { get; }
        object Proxy(object? root = null);
    }
    
    public interface IDiverter<T> : IDiverter  where T : class
    {
        ICallContext<T> CallCtx { get; }
        T Proxy(T? root = null);
        IDiverter<T> SendTo(T target);
        IDiverter<T> AddSendTo(T target);
        IDiverter<T> Reset();
    }
}