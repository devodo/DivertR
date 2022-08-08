using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.DispatchProxy
{
    public class ProxyInvoker<TTarget> : IProxyInvoker where TTarget : class
    {
        private readonly IProxyCall<TTarget> _proxyCall;
        private readonly TTarget _proxy;
        private readonly TTarget? _root;

        public ProxyInvoker(IProxyCall<TTarget> proxyCall, TTarget proxy, TTarget? root)
        {
            _proxyCall = proxyCall;
            _proxy = proxy;
            _root = root;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Invoke(MethodInfo targetMethod, object[] args)
        {
            return _proxyCall.Call(_proxy, _root, targetMethod, args);
        }
    }
}