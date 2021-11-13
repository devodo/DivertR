using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR.Record
{
    public interface IRecordStream<TTarget> : IReadOnlyCollection<IRecordedCall<TTarget>> where TTarget : class
    {
        IEnumerable<IRecordedCall<TTarget>> To(ICallConstraint<TTarget>? callConstraint = null);
        IFuncRecordStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IActionRecordStream<TTarget> To(Expression<Action<TTarget>> lambdaExpression);
        IActionRecordStream<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
    }
}
