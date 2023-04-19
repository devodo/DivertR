using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DivertR.Internal
{
    internal class ViaRedirectTypeCallConstraint : ICallConstraint
    {
        private readonly Type _returnType;
        private readonly Type _returnTaskType;
        private readonly Type _returnValueTaskType;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ViaRedirectTypeCallConstraint(Type returnType)
        {
            _returnType = returnType;
            _returnTaskType = typeof(Task<>).MakeGenericType(returnType);
            _returnValueTaskType = typeof(ValueTask<>).MakeGenericType(returnType);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            if (ReferenceEquals(callInfo.Method.ReturnType, _returnType))
            {
                return true;
            }
            
            if (ReferenceEquals(callInfo.Method.ReturnType, _returnTaskType))
            {
                return true;
            }
            
            return ReferenceEquals(callInfo.Method.ReturnType, _returnValueTaskType);
        }
    }
}