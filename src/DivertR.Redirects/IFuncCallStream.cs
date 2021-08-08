using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DivertR.Redirects
{
    public interface IFuncCallStream<TTarget, TReturn> : IReadOnlyList<IRecordedCall<TTarget, TReturn>> where TTarget : class
    {
        IReadOnlyList<IRecordedCall<TTarget, TReturn, T1>> Visit<T1>(Action<IRecordedCall<TTarget, TReturn, T1>>? visitor = null);
        
        Task<IFuncCallStream<TTarget, TReturn>> Visit<T1>(Func<IRecordedCall<TTarget, TReturn, T1>, Task> visitor);
    }
}
