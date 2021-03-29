using System;

namespace DivertR.Core
{
    public interface IFuncRedirectBuilder<T, TResult> : IRedirectBuilder<T> where T : class
    {
        IVia<T> To(TResult instance);
        IVia<T> To(Func<TResult> redirectDelegate);
        IVia<T> To<T1>(Func<T1, TResult> redirectDelegate);
        IVia<T> To<T1, T2>(Func<T1, T2, TResult> redirectDelegate);
        IVia<T> To<T1, T2, T3>(Func<T1, T2, T3, TResult> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> redirectDelegate);
    }
}