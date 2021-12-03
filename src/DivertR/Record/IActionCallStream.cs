using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IActionCallStream<TTarget> : ICallStream<IRecordedCall<TTarget>> where TTarget : class
    {
        IActionCallStream<TTarget, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        int Replay(Action<IRecordedCall<TTarget>, CallArguments> visitor);
        int Replay(Action<IRecordedCall<TTarget>, CallArguments, int> visitor);
        Task<int> Replay(Func<IRecordedCall<TTarget>, CallArguments, Task> visitor);
        Task<int> Replay(Func<IRecordedCall<TTarget>, CallArguments, int, Task> visitor);
    }

    public interface IActionCallStream<TTarget, out TArgs> : ICallStream<IRecordedCall<TTarget, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IActionCallStream<TTarget, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        int Replay(Action<IRecordedCall<TTarget>, TArgs> visitor);
        int Replay(Action<IRecordedCall<TTarget, TArgs>, TArgs, int> visitor);
        Task<int> Replay(Func<IRecordedCall<TTarget>, TArgs, Task> visitor);
        Task<int> Replay(Func<IRecordedCall<TTarget, TArgs>, TArgs, int, Task> visitor);
    }
}
