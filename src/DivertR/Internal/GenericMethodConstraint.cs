using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class GenericMethodConstraint : IMethodConstraint
    {
        private readonly MethodInfo _genericMethodDefinition;
        private readonly Type[] _genericTypes;

        private readonly ConcurrentDictionary<MethodInfo, bool> _matchCache =
            new ConcurrentDictionary<MethodInfo, bool>();

        public GenericMethodConstraint(MethodInfo methodInfo)
        {
            _genericMethodDefinition = methodInfo.GetGenericMethodDefinition();
            _genericTypes = methodInfo.GetGenericArguments();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(MethodInfo methodInfo)
        {
            return methodInfo.IsGenericMethod && _matchCache.GetOrAdd(methodInfo, IsMatchInternal);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsMatchInternal(MethodInfo methodInfo)
        {
            if (!ReferenceEquals(_genericMethodDefinition, methodInfo.GetGenericMethodDefinition()))
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