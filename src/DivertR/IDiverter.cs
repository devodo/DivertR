using System;
using System.Collections.Generic;

namespace DivertR
{
    public interface IDiverter
    {
        IDiverter Register<TTarget>(string? name = null) where TTarget : class;
        IDiverter Register(Type type, string? name = null);
        IDiverter Register(IEnumerable<Type> types, string? name = null);
        IEnumerable<IVia> RegisteredVias(string? name = null);
        IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class;
        IVia Via(ViaId id);
        IVia Via(Type type, string? name = null);
        IDiverter Strict(string? name = null);
        IDiverter ResetAll();
    }
}