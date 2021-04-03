using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR.Core
{
    public interface ICallRecord<TTarget> where TTarget : class
    {
        IReadOnlyList<IRecordedCall<TTarget>> Calls(ICallConstraint<TTarget>? callConstraint = null);
        IReadOnlyList<IRecordedCall<TTarget>> Calls<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IReadOnlyList<IRecordedCall<TTarget>> Calls(Expression<Action<TTarget>> lambdaExpression);
        IReadOnlyList<IRecordedCall<TTarget>> CallsSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
    }
}