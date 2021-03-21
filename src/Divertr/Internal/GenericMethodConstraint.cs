using System;
using System.Reflection;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class GenericMethodConstraint : IMethodConstraint
    {
        private readonly MethodInfo _methodInfo;
        private readonly Type[] _genericTypes;

        public GenericMethodConstraint(MethodInfo methodInfo, Type[] genericTypes)
        {
            _methodInfo = methodInfo;
            _genericTypes = genericTypes;
        }
        
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