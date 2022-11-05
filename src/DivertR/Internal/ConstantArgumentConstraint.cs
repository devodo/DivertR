using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ConstantArgumentConstraint : IArgumentConstraint
    {
        private readonly object? _constantValue;

        public ConstantArgumentConstraint(ParameterInfo parameter, object? constantValue)
        {
            Parameter = parameter;
            _constantValue = constantValue;
            ArgumentType = _constantValue?.GetType();
        }
        
        public ParameterInfo Parameter { get; }

        public Type? ArgumentType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(object? argument)
        {
            return Equals(_constantValue, argument);
        }
    }
}