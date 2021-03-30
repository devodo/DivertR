using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectBuilder<T> where T : class
    {
        private readonly IVia<T> _via;
        
        private readonly List<ICallConstraint<T>> _callConstraints = new List<ICallConstraint<T>>();
        private int _orderWeight = 0;

        public RedirectBuilder(IVia<T> via)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));
        }

        RedirectBuilder<T> WithOrderWeight(int orderWeight)
        {
            _orderWeight = orderWeight;

            return this;
        }
        
        public ICallConstraint<T> BuildCallConstraint()
        {
            return _callConstraints.Count switch
            {
                0 => TrueCallConstraint<T>.Instance,
                1 => _callConstraints[0],
                _ => new CompositeCallConstraint<T>(_callConstraints)
            };
        }
        
        public IRedirect<T> Build(T target)
        {
            return new TargetRedirect<T>(target, BuildCallConstraint());
        }
        
        public IVia<T> To(T target)
        {
            var redirect = Build(target);
            
            return _via.InsertRedirect(redirect);
        }
        
        public IFuncRedirectBuilder<T, TReturn> When<TReturn>(Expression<Func<T, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new FuncRedirectBuilder<T, TReturn>(_via, parsedCall);
        }
        
        public IActionRedirectBuilder<T> When(Expression<Action<T>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new ActionRedirectBuilder<T>(_via, parsedCall);
        }
        
        public IActionRedirectBuilder<T> WhenSet<TProperty>(Expression<Func<T, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));
            if (valueExpression?.Body == null) throw new ArgumentNullException(nameof(valueExpression));

            if (!(lambdaExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Only property member expressions are valid input to RedirectSet", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, valueExpression.Body);

            return new ActionRedirectBuilder<T>(_via, parsedCall);
        }
    }
}