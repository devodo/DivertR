using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IActionCallLog<TTarget> : ICallLog<IRecordedCall<TTarget>> where TTarget : class
    {
        ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, TMap> mapper);
        ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper);
        
        int Replay(Action<IRecordedCall<TTarget>, CallArguments> visitor);
        int Replay(Action<IRecordedCall<TTarget>, CallArguments, int> visitor);
        Task<int> Replay(Func<IRecordedCall<TTarget>, CallArguments, Task> visitor);
        Task<int> Replay(Func<IRecordedCall<TTarget>, CallArguments, int, Task> visitor);
    }

    public interface IActionCallLog<TTarget, out TArgs> : ICallLog<IRecordedCall<TTarget, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TMap> mapper);
        ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TArgs, TMap> mapper);
        
        int Replay(Action<IRecordedCall<TTarget>, TArgs> visitor);
        int Replay(Action<IRecordedCall<TTarget, TArgs>, TArgs, int> visitor);
        Task<int> Replay(Func<IRecordedCall<TTarget>, TArgs, Task> visitor);
        Task<int> Replay(Func<IRecordedCall<TTarget, TArgs>, TArgs, int, Task> visitor);
    }
}
