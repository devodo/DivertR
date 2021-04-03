using System;

namespace DivertR.Core
{
    public interface IDiverter
    {
        IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class;
        IVia Via(Type type, string? name = null);
        IDiverter ResetAll();
    }
}