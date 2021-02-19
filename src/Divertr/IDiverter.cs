using System;

namespace Divertr
{
    public interface IDiverter
    {
        IRouter<T> Router<T>(string? name = null) where T : class;
        IRouter Router(Type type, string? name = null);
        IDiverter ResetAll();
        
        // Router shortcuts
        T Proxy<T>(T? original = null) where T : class;
        IRouter<T> Redirect<T>(T target) where T : class;
        IRouter<T> AddRedirect<T>(T target) where T : class;
        IRouter<T> Reset<T>() where T : class;
        ICallRelay<T> Relay<T>() where T : class;
    }
}