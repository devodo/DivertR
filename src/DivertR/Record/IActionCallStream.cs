using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IActionCallStream<TTarget> : ICallStream<IRecordedCall<TTarget>> where TTarget : class
    {
        IActionCallStream<TTarget, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper);
        
        IReplayResult Replay(Action<IRecordedCall<TTarget>, CallArguments> visitor);
        IReplayResult Replay(Action<IRecordedCall<TTarget>, CallArguments, int> visitor);
        Task<IReplayResult> Replay(Func<IRecordedCall<TTarget>, CallArguments, Task> visitor);
        Task<IReplayResult> Replay(Func<IRecordedCall<TTarget>, CallArguments, int, Task> visitor);
    }

    public interface IActionCallStream<TTarget, out TArgs> : ICallStream<IRecordedCall<TTarget, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IActionCallStream<TTarget, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TArgs, TMap> mapper);
        
        IReplayResult Replay(Action<IRecordedCall<TTarget>, TArgs> visitor);
        IReplayResult Replay(Action<IRecordedCall<TTarget, TArgs>, TArgs, int> visitor);
        Task<IReplayResult> Replay(Func<IRecordedCall<TTarget>, TArgs, Task> visitor);
        Task<IReplayResult> Replay(Func<IRecordedCall<TTarget, TArgs>, TArgs, int, Task> visitor);
    }
}
