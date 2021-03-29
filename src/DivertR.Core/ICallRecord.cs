using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR.Core
{
    public interface ICallRecord<T> where T : class
    {
        IReadOnlyList<IRecordedCall<T>> Calls(ICallConstraint<T>? callConstraint = null);
        IReadOnlyList<IRecordedCall<T>> Calls<TReturn>(Expression<Func<T, TReturn>> lambdaExpression);
        IReadOnlyList<IRecordedCall<T>> Calls(Expression<Action<T>> lambdaExpression);
        IReadOnlyList<IRecordedCall<T>> CallsSet<TProperty>(Expression<Func<T, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
    }
}