using System;
using System.Linq.Expressions;
using DivertR.Internal;

namespace DivertR
{
    public static class Builder<TTarget> where TTarget : class
    {
        public static IFuncRedirectBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var parsedCall = CallExpressionParser.FromExpression(constraintExpression.Body);
            var callConstraint = parsedCall.CreateCallConstraint<TTarget>();
            
            return new FuncRedirectBuilder<TTarget, TReturn>(parsedCall, callConstraint);
        }
    }

    public static class Calls
    {
        public static IFuncRedirectBuilder<TReturn> Returning<TReturn>(bool matchSubType = false)
        {
            var callValidator = new ReturnCallValidator(typeof(TReturn), matchSubType);
            var callConstraint = callValidator.CreateCallConstraint();
            
            return new FuncRedirectBuilder<TReturn>(callValidator, callConstraint);
        }
    }
}