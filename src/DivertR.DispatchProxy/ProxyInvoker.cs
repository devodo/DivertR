using System.Reflection;
using DivertR.Core;

namespace DivertR.DispatchProxy
{
    internal class ProxyInvoker<T> : IProxyInvoker where T : class
    {
        private readonly IProxyCall<T> _proxyCall;
        private readonly T _proxy;

        public ProxyInvoker(T proxy, IProxyCall<T> proxyCall)
        {
            _proxy = proxy;
            _proxyCall = proxyCall;
        }
        
        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var callInfo = new CallInfo<T>(_proxy, null, targetMethod, args);

            return _proxyCall.Call(callInfo)!;
        }
    }
}