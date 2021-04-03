using System;
using System.Linq.Expressions;

namespace DivertR.Core
{
    public interface IVia
    {
        ViaId ViaId { get; }
        object ProxyObject(object? original = null);
    }
    
    public interface IVia<T> : IVia where T : class
    {
        IRelay<T> Relay { get; }
        T Next { get; }
        
        T Proxy(T? original = null);

        IVia<T> InsertRedirect(IRedirect<T> redirect, int orderWeight = 0);
        IVia<T> Reset();
        
        IVia<T> RedirectTo(T target);
        IRedirectBuilder<T> Redirect(ICallConstraint<T>? callConstraint = null);
        IFuncRedirectBuilder<T, TReturn> Redirect<TReturn>(Expression<Func<T, TReturn>> lambdaExpression);
        IActionRedirectBuilder<T> Redirect(Expression<Action<T>> lambdaExpression);
        IActionRedirectBuilder<T> RedirectSet<TProperty>(Expression<Func<T, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);

        ICallRecord<T> RecordCalls(ICallConstraint<T>? callConstraint = null, int orderWeight = int.MinValue);
    }
}