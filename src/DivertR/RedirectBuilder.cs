using System;
using System.Linq.Expressions;
using DivertR.Internal;

namespace DivertR
{
    public static class RedirectBuilder<TTarget> where TTarget : class?
    {
        public static IRedirectBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            return new Internal.RedirectBuilder<TTarget>(callConstraint);
        }

        public static IFuncRedirectBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromExpression(constraintExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            
            return new FuncRedirectBuilder<TTarget, TReturn>(callValidator, callConstraint.Of<TTarget>());
        }

        public static IActionRedirectBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromExpression(constraintExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            
            return new ActionRedirectBuilder<TTarget>(callValidator, callConstraint.Of<TTarget>());
        }

        public static IActionRedirectBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty?>> constraintExpression)
        {
            if (memberExpression.Body == null) throw new ArgumentNullException(nameof(memberExpression));
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            if (!(memberExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Must be a property member expression", nameof(memberExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, constraintExpression.Body);
            var callConstraint = parsedCall.CreateCallConstraint();

            return new ActionRedirectBuilder<TTarget>(parsedCall, callConstraint.Of<TTarget>());
        }
    }

    public static class RedirectBuilder
    {
        public static IRedirectBuilder To(ICallConstraint? callConstraint = null)
        {
            return new Internal.RedirectBuilder(callConstraint);
        }
        
        public static IFuncRedirectBuilder<TReturn> To<TReturn>(Expression<Func<TReturn>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromExpression(constraintExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            
            return new FuncRedirectBuilder<TReturn>(callConstraint);
        }
    }
}