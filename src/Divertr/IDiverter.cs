using System;

namespace DivertR
{
    public interface IDiverter
    {
        IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class;
        IVia Via(Type type, string? name = null);
        IDiverter ResetAll();
    }
}