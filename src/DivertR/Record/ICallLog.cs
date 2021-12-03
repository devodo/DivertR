using System.Collections.Generic;

namespace DivertR.Record
{
    public interface ICallLog<out TMap> : ICallStream<TMap>, IReadOnlyCollection<TMap>
    {
    }
}