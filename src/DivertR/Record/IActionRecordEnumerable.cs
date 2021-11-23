using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IActionRecordEnumerable<TTarget> : IEnumerable<IActionRecordedCall<TTarget>> where TTarget : class
    {
        IActionRecordEnumerable<TTarget, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IActionRecordEnumerable<TTarget, out TArgs> : IEnumerable<IActionRecordedCall<TTarget, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IActionRecordEnumerable<TTarget, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}
