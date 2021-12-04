using System;
using System.Collections;

namespace DivertR.Record
{
    public interface IFuncCallLog<TTarget, out TReturn> : IFuncCallStream<TTarget, TReturn>, ICallLog<IFuncRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, TMap> mapper);
        ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper);
        
        new IFuncCallLog<TTarget, TReturn, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IFuncCallLog<TTarget, out TReturn, out TArgs> : IFuncCallStream<TTarget, TReturn, TArgs>, ICallLog<IFuncRecordedCall<TTarget, TReturn, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap> mapper);
        ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> mapper);
        
        new IFuncCallLog<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}
