using System;

namespace DivertR.Core
{
    public interface IFuncRedirectBuilder<T, TResult> : IRedirectBuilder<T> where T : class
    {
        IVia<T> To(Func<TResult> redirectDelegate);
        IVia<T> To<T1>(Func<T1, TResult> redirectDelegate);
        IVia<T> To<T1, T2>(Func<T1, T2, TResult> redirectDelegate);
    }
}