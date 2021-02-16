using System;

namespace Divertr
{
    public interface IDiverterCollection
    {
        IDiverter<T> Of<T>(string? name = null) where T : class;
        IDiverter Of(Type type, string? name = null);
        IDiverterCollection ResetAll();
    }
}