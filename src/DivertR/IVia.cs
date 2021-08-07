﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR
{
    public interface IVia
    {
        ViaId ViaId { get; }
        object ProxyObject(object? original = null);
    }
    
    public interface IVia<TTarget> : IVia where TTarget : class
    {
        IRelay<TTarget> Relay { get; }
        TTarget Next { get; }
        IReadOnlyList<IRedirect<TTarget>> ConfiguredRedirects { get; }

        TTarget Proxy(TTarget? original = null);

        IVia<TTarget> InsertRedirect(IRedirect<TTarget> redirect, int orderWeight = 0);
        IVia<TTarget> Reset();
        
        IVia<TTarget> Redirect(TTarget target);
        IRedirectBuilder<TTarget> When(ICallConstraint<TTarget>? callConstraint = null);
        IFuncRedirectBuilder<TTarget, TReturn> When<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IActionRedirectBuilder<TTarget> When(Expression<Action<TTarget>> lambdaExpression);
        IActionRedirectBuilder<TTarget> WhenSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
    }
}