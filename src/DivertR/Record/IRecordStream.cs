using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IRecordStream<TTarget> : ICallStream<IRecordedCall<TTarget>> where TTarget : class?
    {
        IRecordStream<TTarget> To(ICallConstraint<TTarget>? callConstraint = null);
        IFuncCallStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression);
        IActionCallStream<TTarget> To(Expression<Action<TTarget>> lambdaExpression);
        IActionCallStream<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression);
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper);
        IVerifySnapshot<IRecordedCall<TTarget>> Verify(Action<IRecordedCall<TTarget>, CallArguments> visitor);
        Task<IVerifySnapshot<IRecordedCall<TTarget>>> Verify(Func<IRecordedCall<TTarget>, CallArguments, Task> visitor);
    }
    
    public interface IRecordStream : ICallStream<IRecordedCall>
    {
        IRecordStream To(ICallConstraint? callConstraint = null);
        IFuncCallStream<TReturn> To<TReturn>(Expression<Func<TReturn>> constraintExpression);
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall, CallArguments, TMap> mapper);
        IVerifySnapshot<IRecordedCall> Verify(Action<IRecordedCall, CallArguments> visitor);
        Task<IVerifySnapshot<IRecordedCall>> Verify(Func<IRecordedCall, CallArguments, Task> visitor);
    }
}
