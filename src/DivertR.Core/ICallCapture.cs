using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DivertR.Core
{
    public interface ICallCapture<T> where T : class
    {
        List<CapturedCall<T>> Calls(ICallConstraint? callConstraint = null);
        List<CapturedCall<T>> Calls<TReturn>(Expression<Func<T, TReturn>> lambdaExpression);
        List<CapturedCall<T>> Calls(Expression<Action<T>> lambdaExpression);
    }
}