using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RootProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class?
    {
        private readonly IRelay _relay;

        public RootProxyCall(IRelay relay)
        {
            _relay = relay;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call([DisallowNull] TTarget proxy, TTarget? root, MethodInfo method, CallArguments arguments)
        {
            var callInfo = new CallInfo<TTarget>(proxy, root, method, arguments);
            
            return _relay.CallRoot(callInfo.Method, callInfo.Arguments);
        }
    }
}