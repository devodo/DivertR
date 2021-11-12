using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IActionCallStream<TTarget> : IReadOnlyList<IActionRecordedCall<TTarget>> where TTarget : class
    {
        IActionCallStream<TTarget> ForEach(Action<IActionRecordedCall<TTarget>> visitor);
        IActionCallStream<TTarget> ForEach(Action<IActionRecordedCall<TTarget>, int> visitor);
        
        IActionCallStream<TTarget, TArgs> ForEach<TArgs>(Action<IActionRecordedCall<TTarget, TArgs>> visitor)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionCallStream<TTarget, TArgs> ForEach<TArgs>(Action<IActionRecordedCall<TTarget, TArgs>, int> visitor)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionCallStream<TTarget, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IReadOnlyList<TMap> Map<TMap>(Func<IActionRecordedCall<TTarget>, TMap> mapper);
        IReadOnlyList<TMap> Map<TMap>(Func<IActionRecordedCall<TTarget>, int, TMap> mapper);
    }

    public interface IActionCallStream<TTarget, out TArgs> : IReadOnlyList<IActionRecordedCall<TTarget, TArgs>>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IActionCallStream<TTarget, TArgs> ForEach(Action<IActionRecordedCall<TTarget, TArgs>> visitor);
        IActionCallStream<TTarget, TArgs> ForEach(Action<IActionRecordedCall<TTarget, TArgs>, int> visitor);
        IReadOnlyList<TMap> Map<TMap>(Func<IActionRecordedCall<TTarget, TArgs>, TMap> mapper);
        IReadOnlyList<TMap> Map<TMap>(Func<IActionRecordedCall<TTarget, TArgs>, int, TMap> mapper);
    }
}
