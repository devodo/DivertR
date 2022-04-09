using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.DispatchProxy
{
    public class ProxyInvoker<TTarget> : IProxyInvoker where TTarget : class
    {
        private readonly IProxyCall _proxyCall;
        private readonly TTarget _proxy;

        public ProxyInvoker(TTarget proxy, IProxyCall proxyCall)
        {
            _proxy = proxy;
            _proxyCall = proxyCall;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Invoke(MethodInfo targetMethod, object[] args)
        {
            var callInfo = new CallInfo<TTarget>(_proxy, null, targetMethod, args);

            return _proxyCall.Call(callInfo);
        }
    }
}