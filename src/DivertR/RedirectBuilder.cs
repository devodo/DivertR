using System;
using System.Linq.Expressions;
using DivertR.Internal;

namespace DivertR
{
    public static class RedirectBuilder<TTarget> where TTarget : class
    {
        public static IRedirectBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            return new Internal.RedirectBuilder<TTarget>(callConstraint);
        }

        public static IFuncRedirectBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var parsedCall = CallExpressionParser.FromExpression(constraintExpression.Body);
            var callConstraint = parsedCall.CreateCallConstraint<TTarget>();
            
            return new FuncRedirectBuilder<TTarget, TReturn>(parsedCall, callConstraint);
        }

        public static IActionRedirectBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var parsedCall = CallExpressionParser.FromExpression(constraintExpression.Body);
            var callConstraint = parsedCall.CreateCallConstraint<TTarget>();
            
            return new ActionRedirectBuilder<TTarget>(parsedCall, callConstraint);
        }
        
        public static IActionRedirectBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>> constraintExpression)
        {
            if (memberExpression.Body == null) throw new ArgumentNullException(nameof(memberExpression));
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            if (!(memberExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Must be a property member expression", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, constraintExpression.Body);
            var callConstraint = parsedCall.CreateCallConstraint<TTarget>();

            return new ActionRedirectBuilder<TTarget>(parsedCall, callConstraint);
        }
        
        public static IFuncRedirectBuilder<TTarget, TReturn> Returning<TReturn>(bool matchSubType = false) where TReturn : struct
        {
            var callValidator = new ReturnCallValidator(typeof(TReturn), matchSubType);
            var callConstraint = callValidator.CreateCallConstraint<TTarget>();
            
            return new FuncRedirectBuilder<TTarget, TReturn>(callValidator, callConstraint);
        }
    }

    public static class RedirectBuilder
    {
        public static IRedirectBuilder To(ICallConstraint? callConstraint = null)
        {
            return new Internal.RedirectBuilder(callConstraint);
        }
        
        public static IFuncRedirectBuilder<TReturn> Returning<TReturn>(bool matchSubType = false)
        {
            var callValidator = new ReturnCallValidator(typeof(TReturn), matchSubType);
            var callConstraint = callValidator.CreateCallConstraint();
            
            return new FuncRedirectBuilder<TReturn>(callValidator, callConstraint);
        }
    }
}