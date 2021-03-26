using Castle.DynamicProxy;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DynamicProxy
{
    internal class RedirectInterceptor<T> : IInterceptor where T : class
    {
        private readonly IRelayState<T> _relayState;

        public RedirectInterceptor(IRelayState<T> relayState)
        {
            _relayState = relayState;
        }

        public void Intercept(IInvocation invocation)
        {
            var lastCall = _relayState.CallInfo;
            var call = new CallInfo<T>(lastCall.Proxy, lastCall.Original, invocation.Method, invocation.Arguments);
            invocation.ReturnValue = _relayState.CallNext(call);
        }
    }
}