using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class GenericMethodConstraint : IMethodConstraint
    {
        private readonly Type[] _genericTypes;

        public GenericMethodConstraint(Type[] genericTypes)
        {
            _genericTypes = genericTypes;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(MethodInfo methodInfo)
        {
            if (!methodInfo.IsGenericMethod)
            {
                return false;
            }

            var compareTypes = methodInfo.GetGenericArguments();

            if (_genericTypes.Length != compareTypes.Length)
            {
                return false;
            }

            for (var i = 0; i < _genericTypes.Length; i++)
            {
                if (ReferenceEquals(_genericTypes[i], compareTypes[i]))
                {
                    continue;
                }

                if (!_genericTypes[i].IsAssignableFrom(compareTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}