﻿using System;
using DivertR.Core;

namespace DivertR
{
    public interface IRedirectBuilder<T> where T : class
    {
        IVia<T> To(T target, object? state = null);
    }

    public interface IRedirectBuilder<T, TResult> : IRedirectBuilder<T> where T : class
    {
        IVia<T> To<T1>(Func<T1, TResult> redirectDelegate);
        IVia<T> To(Func<TResult> redirectDelegate);
    }
}