using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DivertR.Core
{
    public interface ICallRecord<T> where T : class
    {
        ReadOnlyCollection<IRecordedCall<T>> Calls(ICallConstraint<T>? callConstraint = null);
        ReadOnlyCollection<IRecordedCall<T>> Calls<TReturn>(Expression<Func<T, TReturn>> lambdaExpression);
        ReadOnlyCollection<IRecordedCall<T>> Calls(Expression<Action<T>> lambdaExpression);
    }
}