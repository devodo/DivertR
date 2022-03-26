using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class DefaultRootCallHandler : ICallHandler
    {
        private static readonly ConcurrentDictionary<Type, object> TypeDefaults = new ConcurrentDictionary<Type, object>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo callInfo)
        {
            var returnType = callInfo.Method.ReturnType;
            
            //if (returnType.GetGenericTypeDefinition())
             //   return null;
            
            
            return returnType.IsValueType
                ? TypeDefaults.GetOrAdd(returnType, Activator.CreateInstance)
                : null;
        }
    }
}