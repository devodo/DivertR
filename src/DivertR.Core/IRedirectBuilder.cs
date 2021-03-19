using System;

namespace DivertR.Core
{
    public interface IRedirectBuilder<T> where T : class
    {
        IVia<T> To(T target, object? state = null);
        IVia<T> To(Delegate redirectDelegate);
    }

    public interface IActionRedirectBuilder<T> : IRedirectBuilder<T> where T : class
    {
        IVia<T> To<T1>(Action<T1> redirectDelegate);
    }

    public interface IFuncRedirectBuilder<T, TResult> : IRedirectBuilder<T> where T : class
    {
        IVia<T> To(Func<TResult> redirectDelegate);
        IVia<T> To<T1>(Func<T1, TResult> redirectDelegate);
        IVia<T> To<T1, T2>(Func<T1, T2, TResult> redirectDelegate);
    }
}