using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR.Core
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
        IReadOnlyList<IRedirect<TTarget>> Redirects { get; }

        TTarget Proxy(TTarget? original = null);

        IVia<TTarget> InsertRedirect(IRedirect<TTarget> redirect, int orderWeight = 0);
        IVia<TTarget> Reset();
        
        IVia<TTarget> RedirectTo(TTarget target);
        IRedirectBuilder<TTarget> Redirect(ICallConstraint<TTarget>? callConstraint = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IActionRedirectBuilder<TTarget> Redirect(Expression<Action<TTarget>> lambdaExpression);
        IActionRedirectBuilder<TTarget> RedirectSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);

        ICallRecord<TTarget> RecordCalls(ICallConstraint<TTarget>? callConstraint = null, int orderWeight = int.MinValue);
    }
}