using System;
using System.Collections.Generic;

namespace DivertR.Redirects
{
    public interface IFuncCallStream<TTarget, TReturn> : IReadOnlyCollection<IRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        IFuncCallStream<TTarget, TReturn> Visit<T1>(Action<T1, ICallReturn<TReturn>?> verifyDelegate);
    }
}
