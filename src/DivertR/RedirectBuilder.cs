﻿using System;
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

            var callValidator = CallExpressionParser.FromExpression<TTarget>(constraintExpression.Body);
            var callConstraint = new CallConstraintWrapper<TTarget>(callValidator.CreateCallConstraint());
            
            return new FuncRedirectBuilder<TTarget, TReturn>(callValidator, callConstraint);
        }

        public static IActionRedirectBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromExpression<TTarget>(constraintExpression.Body);
            var callConstraint = new CallConstraintWrapper<TTarget>(callValidator.CreateCallConstraint());
            
            return new ActionRedirectBuilder<TTarget>(callValidator, callConstraint);
        }

        public static IActionRedirectBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null)
        {
            if (memberExpression.Body == null) throw new ArgumentNullException(nameof(memberExpression));
            if (constraintExpression is { Body: null }) throw new ArgumentNullException(nameof(constraintExpression));

            if (!(memberExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Must be a property member expression", nameof(memberExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, constraintExpression?.Body);
            var callConstraint = new CallConstraintWrapper<TTarget>(parsedCall.CreateCallConstraint());

            return new ActionRedirectBuilder<TTarget>(parsedCall, callConstraint);
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

            var callValidator = CallExpressionParser.FromProperty(constraintExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            
            return new FuncRedirectBuilder<TReturn>(callConstraint);
        }
    }
}