using System;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IActionCallStream<TTarget> : IReadOnlyList<IActionRecordedCall<TTarget>> where TTarget : class
    {
        IActionCallStream<TTarget> ForEach(Action<IActionRecordedCall<TTarget>> visitor);
        IActionCallStream<TTarget> ForEach(Action<IActionRecordedCall<TTarget>, int> visitor);
    }
}
