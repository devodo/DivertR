using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IFuncCallStream<TTarget, out TReturn> : IReadOnlyList<IFuncRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        IFuncCallStream<TTarget, TReturn> ForEach(Action<IFuncRecordedCall<TTarget, TReturn>> visitor);
        IFuncCallStream<TTarget, TReturn> ForEach(Action<IFuncRecordedCall<TTarget, TReturn>, int> visitor);

        IFuncCallStream<TTarget, TReturn, TArgs> ForEach<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncCallStream<TTarget, TReturn, TArgs> ForEach<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, int> visitor)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncCallStream<TTarget, TReturn, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IReadOnlyList<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, TMap> mapper);
        IReadOnlyList<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, int, TMap> mapper);
    }
    
    public interface IFuncCallStream<TTarget, out TReturn, out TArgs> : IReadOnlyList<IFuncRecordedCall<TTarget, TReturn, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IFuncCallStream<TTarget, TReturn, TArgs> ForEach(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor);
        IFuncCallStream<TTarget, TReturn, TArgs> ForEach(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, int> visitor);
        -
    }
}
