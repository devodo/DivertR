using System;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IActionCallStream<TTarget> : IReadOnlyList<IRecordedCall<TTarget>> where TTarget : class
    {
        IReadOnlyList<IRecordedCall<TTarget>> Visit(Action<IRecordedCall<TTarget>>? visitor = null);
        IReadOnlyList<IRecordedCall<TTarget, T1>> Visit<T1>(Action<IRecordedCall<TTarget, T1>>? visitor = null);
        IReadOnlyList<IRecordedCall<TTarget, T1>> Visit<T1>(Action<IRecordedCall<TTarget, T1>, T1> visitor);
    }
}
