using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class TypeArgumentConstraint<TArgument> : IArgumentConstraint
    {
        public TypeArgumentConstraint(ParameterInfo parameter)
        {
            Parameter = parameter;
        }

        public ParameterInfo Parameter { get; }
        public Type ArgumentType => typeof(TArgument);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(object? argument)
        {
            if (argument == null)
            {
                return true;
            }

            return argument is TArgument;
        }
    }
}