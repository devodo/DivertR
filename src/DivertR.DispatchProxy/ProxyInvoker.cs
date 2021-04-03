using System.Reflection;
using DivertR.Core;

namespace DivertR.DispatchProxy
{
    internal class ProxyInvoker<TTarget> : IProxyInvoker where TTarget : class
    {
        private readonly IProxyCall<TTarget> _proxyCall;
        private readonly TTarget _proxy;

        public ProxyInvoker(TTarget proxy, IProxyCall<TTarget> proxyCall)
        {
            _proxy = proxy;
            _proxyCall = proxyCall;
        }
        
        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var callInfo = new CallInfo<TTarget>(_proxy, null, targetMethod, args);

            return _proxyCall.Call(callInfo)!;
        }
    }
}