using System;
using System.Linq.Expressions;

namespace DivertR.Internal
{
    internal class LambdaArgumentConstraint<TTarget> : IArgumentConstraint
    {
        private readonly Func<TTarget, bool> _matchFunc;

        public LambdaArgumentConstraint(LambdaExpression lambdaExpression)
        {
            _matchFunc = (Func<TTarget, bool>) lambdaExpression.Compile();
        }

        public bool IsMatch(object? argument)
        {
            return _matchFunc((TTarget) argument!);
        }
    }
}