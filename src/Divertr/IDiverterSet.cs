using System;

namespace Divertr
{
    public interface IDiverterSet
    {
        T Proxy<T>(T? origin = null, string? name = null) where T : class;
        IDiverter<T> Get<T>(string? name = null) where T : class;
        IDiverter<T> Redirect<T>(T target, string? name = null) where T : class;
        ICallContext<T> CallCtx<T>(string? name = null) where T : class;
        IDiverter Get(Type type, string? name = null);
        void ResetAll();
    }
}