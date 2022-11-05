using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal interface IArgumentConstraint
    {
        ParameterInfo Parameter { get; }
        Type? ArgumentType { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsMatch(object? argument);
    }
}