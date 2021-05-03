using System;

namespace DivertR
{
    public interface IActionRedirectBuilder<TTarget> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        IActionRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IActionRedirectBuilder<TTarget> WithOrderWeight(int orderWeight);
        IActionRedirectBuilder<TTarget> AddRedirectDecorator(Func<IRedirect<TTarget>, IRedirect<TTarget>> decorator);
        
        IRedirect<TTarget> Build(Action redirectDelegate);
        IRedirect<TTarget> Build<T1>(Action<T1> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2>(Action<T1, T2> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate);
        
        IVia<TTarget> To(Action redirectDelegate);
        IVia<TTarget> To<T1>(Action<T1> redirectDelegate);
        IVia<TTarget> To<T1, T2>(Action<T1, T2> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate);
    }
}
