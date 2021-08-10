using System;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IFuncCallStream<TTarget, out TReturn> : IReadOnlyList<IRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        IReadOnlyList<IRecordedCall<TTarget, TReturn, T1>> Visit<T1>(Action<IRecordedCall<TTarget, TReturn, T1>>? visitor = null);
        IReadOnlyList<IRecordedCall<TTarget, TReturn, T1>> Visit<T1>(Action<IRecordedCall<TTarget, TReturn, T1>, T1> visitor);
    }
}
