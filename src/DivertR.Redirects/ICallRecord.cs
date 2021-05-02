using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR.Redirects
{
    public interface ICallRecord<TTarget> : IReadOnlyCollection<IRecordedCall<TTarget>> where TTarget : class
    {
        IReadOnlyList<IRecordedCall<TTarget>> When(ICallConstraint<TTarget>? callConstraint = null);
        IReadOnlyList<IRecordedCall<TTarget, TReturn>> When<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IReadOnlyList<IRecordedCall<TTarget>> When(Expression<Action<TTarget>> lambdaExpression);
        IReadOnlyList<IRecordedCall<TTarget>> WhenSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
    }
}
