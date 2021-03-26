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
        IVia<T> RedirectTo(T target);
        IVia<T> AddRedirect(IRedirect<T> redirect);
        IVia<T> InsertRedirect(int index, IRedirect<T> redirect);
        IVia<T> Reset();
        IRedirectBuilder<T> Redirect(ICallConstraint<T>? callConstraint = null);
        IFuncRedirectBuilder<T, TReturn> Redirect<TReturn>(Expression<Func<T, TReturn>> lambdaExpression);
        IActionRedirectBuilder<T> Redirect(Expression<Action<T>> lambdaExpression);
        IActionRedirectBuilder<T> RedirectSet<TProperty>(Expression<Func<T, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);

        ICallCapture<T> CaptureCalls(ICallConstraint<T>? callConstraint = null);
    }
}