using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ReturnCallConstraint : ICallConstraint
    {
        private readonly Type _returnType;
        private readonly bool _matchSubType;

        public ReturnCallConstraint(Type returnType, bool matchSubType)
        {
            _returnType = returnType;
            _matchSubType = matchSubType;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            return ReturnTypeValid(callInfo.Method.ReturnType);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ReturnTypeValid(Type callReturnType)
        {
            if (ReferenceEquals(callReturnType, _returnType))
            {
                return true;
            }
            
            return _matchSubType && _returnType.IsAssignableFrom(callReturnType);
        }
    }
}
