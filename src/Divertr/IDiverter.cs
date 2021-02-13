using System;
using System.Collections.Generic;

namespace Divertr
{
    public interface IDiverter
    {
        IDiversion<T> Of<T>(string? name = null) where T : class;
        IDiversion Of(Type type, string? name = null);
        IDiversion<T> Redirect<T>(T target, string? name = null) where T : class;
        IDiversion<T> Reset<T>(string? name = null) where T : class;
        IDiverter ResetAll();
        
        T Proxy<T>(T? origin = null, string? name = null) where T : class;
        
        ICallContext<T> CallCtx<T>(string? name = null) where T : class;
        IEnumerable<Type> KnownTypes(string? name = null);
    }
}