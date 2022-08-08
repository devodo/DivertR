using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public int GetHashCode(T value)
        {
            return RuntimeHelpers.GetHashCode(value);
        }

        public bool Equals(T left, T right)
        {
            return ReferenceEquals(left, right);
        }
    }
}