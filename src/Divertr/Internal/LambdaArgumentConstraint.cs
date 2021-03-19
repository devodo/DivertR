using System;
using System.Linq.Expressions;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class LambdaArgumentConstraint<T> : IArgumentConstraint
    {
        private readonly Func<T, bool> _matchFunc;

        public LambdaArgumentConstraint(LambdaExpression lambdaExpression)
        {
            _matchFunc = (Func<T, bool>) lambdaExpression.Compile();
        }

        public bool IsMatch(object? argument)
        {
            return _matchFunc((T) argument!);
        }
    }
}