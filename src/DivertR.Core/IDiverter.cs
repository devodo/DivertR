using System;

namespace DivertR.Core
{
    public interface IDiverter
    {
        IVia<T> Via<T>(string? name = null) where T : class;
        IVia Via(Type type, string? name = null);
        IDiverter ResetAll();
        
        // Router shortcuts
        T Proxy<T>(T? original = null) where T : class;
        IVia<T> Redirect<T>(T target) where T : class;
        IVia<T> AddRedirect<T>(T target) where T : class;
        IVia<T> Reset<T>() where T : class;
        IRelay<T> Relay<T>() where T : class;
    }
}