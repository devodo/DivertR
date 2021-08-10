using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class LambdaArgumentConstraint<TTarget> : IArgumentConstraint
    {
        private readonly Func<TTarget, bool> _matchFunc;

        public LambdaArgumentConstraint(LambdaExpression lambdaExpression)
        {
            _matchFunc = (Func<TTarget, bool>) lambdaExpression.Compile();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(object? argument)
        {
            return _matchFunc((TTarget) argument!);
        }
    }
}