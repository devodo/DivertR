using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR.Redirects
{
    public interface ICallStream<TTarget> : IReadOnlyCollection<IRecordedCall<TTarget>> where TTarget : class
    {
        IReadOnlyList<IRecordedCall<TTarget>> To(ICallConstraint<TTarget>? callConstraint = null);
        IFuncCallStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IReadOnlyList<IRecordedCall<TTarget>> To(Expression<Action<TTarget>> lambdaExpression);
        IReadOnlyList<IRecordedCall<TTarget>> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
    }
}
