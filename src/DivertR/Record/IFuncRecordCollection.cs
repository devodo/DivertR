using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IFuncRecordCollection<TTarget, out TReturn> : IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        IFuncRecordCollection<TTarget, TReturn, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IFuncRecordCollection<TTarget, out TReturn, out TArgs> : IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IFuncRecordCollection<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}
