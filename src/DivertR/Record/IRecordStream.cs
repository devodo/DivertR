using System;
using System.Linq.Expressions;

namespace DivertR.Record
{
    public interface IRecordStream<TTarget> : ICallStream<IRecordedCall<TTarget>> where TTarget : class?
    {
        IRecordStream<TTarget> To(ICallConstraint<TTarget>? callConstraint = null);
        IFuncCallStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IActionCallStream<TTarget> To(Expression<Action<TTarget>> lambdaExpression);
        IActionCallStream<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
    }
    
    public interface IRecordStream : ICallStream<IRecordedCall>
    {
        IRecordStream To(ICallConstraint? callConstraint = null);
        IFuncCallStream<TReturn> To<TReturn>(Expression<Func<TReturn>> constraintExpression);
    }
}
