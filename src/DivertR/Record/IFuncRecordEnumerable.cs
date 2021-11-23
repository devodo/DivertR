using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IFuncRecordEnumerable<TTarget, out TReturn> : IEnumerable<IFuncRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        IFuncRecordEnumerable<TTarget, TReturn, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IFuncRecordEnumerable<TTarget, out TReturn, out TArgs> : IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IFuncRecordEnumerable<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}
