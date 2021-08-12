﻿using System;

namespace DivertR
{
    public interface IFuncRedirectBuilder<TTarget, TReturn> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint);
        new IFuncRedirectBuilder<TTarget, TReturn> Chain(Func<IVia<TTarget>, IRedirect<TTarget>, IRedirect<TTarget>> chainLink);
        
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
        
        IVia<TTarget> Redirect(TReturn instance, int orderWeight = 0);
        IVia<TTarget> Redirect(Func<TReturn> redirectDelegate, int orderWeight = 0);
        IVia<TTarget> Redirect<T1>(Func<T1, TReturn> redirectDelegate, int orderWeight = 0);
        IVia<TTarget> Redirect<T1, T2>(Func<T1, T2, TReturn> redirectDelegate, int orderWeight = 0);
        IVia<TTarget> Redirect<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate, int orderWeight = 0);
        IVia<TTarget> Redirect<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate, int orderWeight = 0);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate, int orderWeight = 0);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate, int orderWeight = 0);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate, int orderWeight = 0);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate, int orderWeight = 0);
    }
}
