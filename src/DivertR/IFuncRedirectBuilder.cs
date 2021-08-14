using System;

namespace DivertR
{
    public interface IFuncRedirectBuilder<TTarget, in TReturn> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint);
        new IFuncRedirectBuilder<TTarget, TReturn> ChainCallHandler(Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>> chainLink);
        new IFuncRedirectBuilder<TTarget, TReturn> WithOrderWeight(int orderWeight);
        new IFuncRedirectBuilder<TTarget, TReturn> WithExcludeStrict(bool excludeStrict = true);
        
        Redirect<TTarget> Build(TReturn instance);
        Redirect<TTarget> Build(Func<TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1>(Func<T1, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2>(Func<T1, T2, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate);
        
        IVia<TTarget> Redirect(TReturn instance);
        IVia<TTarget> Redirect(Func<TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1>(Func<T1, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2>(Func<T1, T2, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate);
    }
}
