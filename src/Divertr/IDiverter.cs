using System;
using System.Collections.Generic;

namespace Divertr
{
    public interface IDiverter
    {
        IDiversion<T> Of<T>(string? name = null) where T : class;
        IDiversion Of(Type type, string? name = null);
        IDiverter ResetAll();
        IEnumerable<Type> KnownTypes(string? name = null);
    }
}