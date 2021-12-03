using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface ICallStream<out TMap> : IEnumerable<TMap>
    {
        int Replay(Action<TMap> visitor);
        int Replay(Action<TMap, int> visitor);
        Task<int> Replay(Func<TMap, Task> visitor);
        Task<int> Replay(Func<TMap, int, Task> visitor);
    }
}