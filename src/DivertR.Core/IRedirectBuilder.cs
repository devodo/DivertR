using System;
using System.Linq.Expressions;

namespace DivertR.Core
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        ICallConstraint<TTarget> BuildCallConstraint();
        IRedirect<TTarget> Build(TTarget target);
        IVia<TTarget> To(TTarget target);
        IFuncRedirectBuilder<TTarget, TReturn> When<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IActionRedirectBuilder<TTarget> When(Expression<Action<TTarget>> lambdaExpression);
        IActionRedirectBuilder<TTarget> WhenSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirectBuilder<TTarget> WithOrderWeight(int orderWeight);
    }
}