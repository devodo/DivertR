using System;

namespace DivertR.Core
{
    public interface IActionRedirectBuilder<T> : IRedirectBuilder<T> where T : class
    {
        IVia<T> To(Action redirectDelegate);
        IVia<T> To<T1>(Action<T1> redirectDelegate);
        IVia<T> To<T1, T2>(Action<T1, T2> redirectDelegate);
        IVia<T> To<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate);
        IVia<T> To<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate);
    }
}