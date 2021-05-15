using System;

namespace DivertR.Internal
{
    internal class ValueArgumentConstraint : IArgumentConstraint
    {
        public Func<object> GetValueFunc { get; }

        public ValueArgumentConstraint(Func<object> getValueFunc)
        {
            GetValueFunc = getValueFunc;
        }
        
        public bool IsMatch(object? argument)
        {
            return Equals(GetValueFunc.Invoke(), argument);
        }
    }
}