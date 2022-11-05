using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class LambdaArgumentConstraint<TArgument> : IArgumentConstraint
    {
        private readonly Func<TArgument, bool> _matchFunc;

        public LambdaArgumentConstraint(ParameterInfo parameter, LambdaExpression lambdaExpression)
        {
            Parameter = parameter;
            _matchFunc = (Func<TArgument, bool>) lambdaExpression.Compile();
        }
        
        public ParameterInfo Parameter { get; }

        public Type ArgumentType => typeof(TArgument);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(object? argument)
        {
            return argument switch
            {
                null => _matchFunc(default!),
                TArgument tArg => _matchFunc(tArg),
                _ => false
            };
        }
    }
}