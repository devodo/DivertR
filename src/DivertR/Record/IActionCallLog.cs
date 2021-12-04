using System;
using System.Collections;

namespace DivertR.Record
{
    public interface IActionCallLog<TTarget> : IActionCallStream<TTarget>, ICallLog<IRecordedCall<TTarget>> where TTarget : class
    {
        ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, TMap> mapper);
        ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper);
        
        new IActionCallLog<TTarget, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IActionCallLog<TTarget, out TArgs> : IActionCallStream<TTarget, TArgs>, ICallLog<IRecordedCall<TTarget, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TMap> mapper);
        ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TArgs, TMap> mapper);
        
        new IActionCallLog<TTarget, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}
