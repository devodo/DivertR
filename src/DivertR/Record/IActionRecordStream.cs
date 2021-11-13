using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IActionRecordStream<TTarget> : IEnumerable<IActionRecordedCall<TTarget>> where TTarget : class
    {
        IActionRecordStream<TTarget, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IActionRecordStream<TTarget, out TArgs> : IEnumerable<IActionRecordedCall<TTarget, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IActionRecordStream<TTarget, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}
