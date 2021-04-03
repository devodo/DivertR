using System;
using System.Linq.Expressions;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly RedirectBuilderOptions<TTarget> _builderOptions = new RedirectBuilderOptions<TTarget>();

        public RedirectBuilder(IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));

            if (callConstraint != null)
            {
                _builderOptions.CallConstraints.Add(callConstraint);
            }
        }

        public IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            _builderOptions.CallConstraints.Add(callConstraint);

            return this;
        }

        public IRedirectBuilder<TTarget> WithOrderWeight(int orderWeight)
        {
            _builderOptions.OrderWeight = orderWeight;

            return this;
        }
        
        public ICallConstraint<TTarget> BuildCallConstraint()
        {
            return _builderOptions.CallConstraints.Count switch
            {
                0 => TrueCallConstraint<TTarget>.Instance,
                1 => _builderOptions.CallConstraints[0],
                _ => new CompositeCallConstraint<TTarget>(_builderOptions.CallConstraints)
            };
        }
        
        public IRedirect<TTarget> Build(TTarget target)
        {
            return new TargetRedirect<TTarget>(target, BuildCallConstraint());
        }
        
        public IVia<TTarget> To(TTarget target)
        {
            var redirect = Build(target);
            
            return _via.InsertRedirect(redirect);
        }
        
        public IFuncRedirectBuilder<TTarget, TReturn> When<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new FuncRedirectBuilder<TTarget, TReturn>(_via, parsedCall, _builderOptions);
        }
        
        public IActionRedirectBuilder<TTarget> When(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new ActionRedirectBuilder<TTarget>(_via, parsedCall, _builderOptions);
        }
        
        public IActionRedirectBuilder<TTarget> WhenSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));
            if (valueExpression?.Body == null) throw new ArgumentNullException(nameof(valueExpression));

            if (!(lambdaExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Only property member expressions are valid input to RedirectSet", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, valueExpression.Body);

            return new ActionRedirectBuilder<TTarget>(_via, parsedCall, _builderOptions);
        }
    }
}