using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR.Record
{
    public interface IRecordStream<TTarget> : IReadOnlyCollection<IRecordedCall<TTarget>> where TTarget : class
    {
        IEnumerable<IRecordedCall<TTarget>> To(ICallConstraint<TTarget>? callConstraint = null);
        IFuncCallStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IActionCallStream<TTarget> To(Expression<Action<TTarget>> lambdaExpression);
        IActionCallStream<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, TMap> mapper);
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper);
    }
    
    public interface IRecordStream : IReadOnlyCollection<IRecordedCall>
    {
        IEnumerable<IRecordedCall> To(ICallConstraint? callConstraint = null);
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall, TMap> mapper);
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall, CallArguments, TMap> mapper);
    }
}
