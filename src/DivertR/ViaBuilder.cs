using System;
using System.Linq.Expressions;
using DivertR.Internal;

namespace DivertR
{
    public static class ViaBuilder<TTarget> where TTarget : class?
    {
        public static IViaBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            return new Internal.ViaBuilder<TTarget>(callConstraint);
        }

        public static IFuncViaBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromExpression<TTarget>(constraintExpression.Body);
            var callConstraint = new CallConstraintWrapper<TTarget>(callValidator.CreateCallConstraint());
            
            return new FuncViaBuilder<TTarget, TReturn>(callValidator, callConstraint);
        }

        public static IActionViaBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromExpression<TTarget>(constraintExpression.Body);
            var callConstraint = new CallConstraintWrapper<TTarget>(callValidator.CreateCallConstraint());
            
            return new ActionViaBuilder<TTarget>(callValidator, callConstraint);
        }

        public static IActionViaBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null)
        {
            if (memberExpression.Body == null) throw new ArgumentNullException(nameof(memberExpression));

            constraintExpression ??= (() => Is<TProperty>.Any);
            if (constraintExpression is { Body: null }) throw new ArgumentNullException(nameof(constraintExpression));

            if (!(memberExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Must be a property member expression", nameof(memberExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter<TTarget>(propertyExpression, constraintExpression.Body);
            var callConstraint = new CallConstraintWrapper<TTarget>(parsedCall.CreateCallConstraint());

            return new ActionViaBuilder<TTarget>(parsedCall, callConstraint);
        }
    }

    public static class ViaBuilder
    {
        public static IViaBuilder To(ICallConstraint? callConstraint = null)
        {
            return new Internal.ViaBuilder(callConstraint);
        }
        
        public static IFuncViaBuilder<TReturn> To<TReturn>(Expression<Func<TReturn>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromProperty(constraintExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            
            return new FuncViaBuilder<TReturn>(callConstraint);
        }
    }
}