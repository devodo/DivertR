using System;
using System.Linq.Expressions;
using DivertR.Record;

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
        RedirectPlan<TTarget> RedirectPlan { get; }

        TTarget Proxy(TTarget? original = null);
        
        IVia<TTarget> InsertRedirect(Redirect<TTarget> redirect);
        IVia<TTarget> Reset();
        
        IVia<TTarget> Retarget(TTarget target);
        IRedirectBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null);
        IFuncRedirectBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IActionRedirectBuilder<TTarget> To(Expression<Action<TTarget>> lambdaExpression);
        IActionRedirectBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
        
        IVia<TTarget> Strict();
        ICallStream<TTarget> Record(ICallConstraint<TTarget>? callConstraint = null);
    }
}