using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface ICallStream<out T> : IEnumerable<T>
    {
        ICallStream<TMap> Map<TMap>(Func<T, TMap> mapper);
        
        IReplayResult Replay(Action<T> visitor);
        IReplayResult Replay(Action<T, int> visitor);
        Task<IReplayResult> Replay(Func<T, Task> visitor);
        Task<IReplayResult> Replay(Func<T, int, Task> visitor);
    }
}