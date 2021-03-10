using System;
using System.Linq.Expressions;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class LambdaArgumentCondition<T> : IArgumentCondition
    {
        private readonly Func<T, bool> _matchFunc;

        public LambdaArgumentCondition(LambdaExpression lambdaExpression)
        {
            _matchFunc = (Func<T, bool>) lambdaExpression.Compile();
        }

        public bool IsMatch(object? argument)
        {
            return _matchFunc((T) argument!);
        }
    }
}