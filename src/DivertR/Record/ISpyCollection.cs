using System.Collections.Generic;

namespace DivertR.Record
{
    public interface ISpyCollection<out TMap> : IReadOnlyCollection<TMap>, ISpyEnumerable<TMap>
    {
    }
}