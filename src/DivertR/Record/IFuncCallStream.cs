﻿using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IFuncCallStream<TTarget, out TReturn> : ICallStream<IFuncRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        IFuncCallStream<TTarget, TReturn, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper);

        IReplayResult Replay(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments> visitor);
        IReplayResult Replay(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments, int> visitor);
        Task<IReplayResult> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, Task> visitor);
        Task<IReplayResult> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, int, Task> visitor);
    }
    
    public interface IFuncCallStream<TTarget, out TReturn, out TArgs> : ICallStream<IFuncRecordedCall<TTarget, TReturn, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IFuncCallStream<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> mapper);

        IReplayResult Replay(Action<IFuncRecordedCall<TTarget, TReturn>, TArgs> visitor);
        IReplayResult Replay(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int> visitor);
        Task<IReplayResult> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, TArgs, Task> visitor);
        Task<IReplayResult> Replay(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int, Task> visitor);
    }
}
