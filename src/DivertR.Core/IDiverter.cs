using System;

namespace DivertR.Core
{
    public interface IDiverter
    {
        IVia<T> Via<T>(string? name = null) where T : class;
        IVia Via(Type type, string? name = null);
        IDiverter ResetAll();
    }
}