using System;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IFuncCallStream<TTarget, out TReturn> : IReadOnlyList<IFuncRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        IFuncCallStream<TTarget, TReturn> ForEach(Action<IFuncRecordedCall<TTarget, TReturn>> visitor);
        IFuncCallStream<TTarget, TReturn> ForEach(Action<IFuncRecordedCall<TTarget, TReturn>, int> visitor);
    }
}
