using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IActionRecordCollection<TTarget> : IReadOnlyCollection<IActionRecordedCall<TTarget>> where TTarget : class
    {
        IActionRecordCollection<TTarget, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IActionRecordCollection<TTarget, out TArgs> : IReadOnlyCollection<IActionRecordedCall<TTarget, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IActionRecordCollection<TTarget, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}
