using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR.Redirects
{
    public interface ICallsRecord<TTarget> where TTarget : class
    {
        public IReadOnlyList<RecordedCall<TTarget>> All { get; }
        IReadOnlyList<RecordedCall<TTarget>> Matching(ICallConstraint<TTarget> callConstraint);
        IReadOnlyList<RecordedCall<TTarget, TReturn>> Matching<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IReadOnlyList<RecordedCall<TTarget>> Matching(Expression<Action<TTarget>> lambdaExpression);
        IReadOnlyList<RecordedCall<TTarget>> MatchSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
    }
}