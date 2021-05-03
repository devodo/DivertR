using System;

namespace DivertR
{
    public interface IFuncRedirectBuilder<TTarget, TReturn> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint);
        new IFuncRedirectBuilder<TTarget, TReturn> WithOrderWeight(int orderWeight);
        new IFuncRedirectBuilder<TTarget, TReturn> AddRedirectDecorator(Func<IRedirect<TTarget>, IRedirect<TTarget>> decorator);
        
        IRedirect<TTarget> Build(TReturn instance);
        IRedirect<TTarget> Build(Func<TReturn> redirectDelegate);
        IRedirect<TTarget> Build<T1>(Func<T1, TReturn> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2>(Func<T1, T2, TReturn> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate);
        IRedirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate);
        
        IVia<TTarget> To(TReturn instance);
        IVia<TTarget> To(Func<TReturn> redirectDelegate);
        IVia<TTarget> To<T1>(Func<T1, TReturn> redirectDelegate);
        IVia<TTarget> To<T1, T2>(Func<T1, T2, TReturn> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate);
        IVia<TTarget> To<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate);
    }
}
