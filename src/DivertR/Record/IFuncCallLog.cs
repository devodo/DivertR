using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IFuncCallLog<TTarget, out TReturn> : ICallLog<IFuncRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, TMap> mapper);
        ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper);
        
        int Replay(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments> visitor);
        int Replay(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments, int> visitor);
        Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, Task> visitor);
        Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, int, Task> visitor);
    }
    
    public interface IFuncCallLog<TTarget, out TReturn, out TArgs> : ICallLog<IFuncRecordedCall<TTarget, TReturn, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap> mapper);
        ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> mapper);
        
        int Replay(Action<IFuncRecordedCall<TTarget, TReturn>, TArgs> visitor);
        int Replay(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int> visitor);
        Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, TArgs, Task> visitor);
        Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int, Task> visitor);
    }
}
