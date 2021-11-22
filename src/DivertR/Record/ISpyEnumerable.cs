using System;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface ISpyEnumerable<out TMap> : IEnumerable<TMap>
    {
        ISpyEnumerable<TMap> ForEach(Action<TMap> visitor);

        ISpyEnumerable<TMap> ForEach(Action<TMap, int> visitor);
    }
}