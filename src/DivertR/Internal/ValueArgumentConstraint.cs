using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ValueArgumentConstraint : IArgumentConstraint
    {
        public Func<object> GetValueFunc { get; }

        public ValueArgumentConstraint(Func<object> getValueFunc)
        {
            GetValueFunc = getValueFunc;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(object? argument)
        {
            return Equals(GetValueFunc.Invoke(), argument);
        }
    }
}