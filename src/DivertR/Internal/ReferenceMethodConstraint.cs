﻿using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ReferenceMethodConstraint : IMethodConstraint
    {
        private readonly MethodInfo _methodInfo;

        public ReferenceMethodConstraint(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(MethodInfo methodInfo)
        {
            return ReferenceEquals(_methodInfo, methodInfo);
        }
    }
}