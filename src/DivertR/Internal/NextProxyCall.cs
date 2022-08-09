using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class NextProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class?
    {
        private readonly IRelay _relay;

        public NextProxyCall(IRelay relay)
        {
            _relay = relay;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(TTarget proxy, TTarget? root, MethodInfo method, CallArguments arguments)
        {
            var callInfo = new CallInfo<TTarget>(proxy, root, method, arguments);
            
            return _relay.CallNext(callInfo.Method, callInfo.Arguments);
        }
    }
}