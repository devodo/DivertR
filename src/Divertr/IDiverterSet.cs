using System;

namespace Divertr
{
    public interface IDiverterSet
    {
        IDiverter<T> Get<T>(string? name = null) where T : class;
        IDiverter Get(Type type, string? name = null);
        void ResetAll();
    }
}