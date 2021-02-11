using System;
using System.Collections.Generic;

namespace Divertr
{
    public interface IDiverter
    {
        IDirector<T> For<T>(string? name = null) where T : class;
        void ResetAll();
        
        T Proxy<T>(T? origin = null, string? name = null) where T : class;
        IDirector<T> Redirect<T>(T target, string? name = null) where T : class;
        ICallContext<T> CallCtx<T>(string? name = null) where T : class;
        IDirector For(Type type, string? name = null);
        
        IEnumerable<Type> KnownTypes(string? name = null);
    }
}