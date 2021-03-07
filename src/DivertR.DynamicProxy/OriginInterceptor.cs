using Castle.DynamicProxy;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DynamicProxy
{
    internal class OriginInterceptor<T> : IInterceptor where T : class
    {
        private readonly IRelayState<T> _relayState;

        public OriginInterceptor(IRelayState<T> relayState)
        {
            _relayState = relayState;
        }

        public void Intercept(IInvocation invocation)
        {
            var original = _relayState.Original;
            
            if (original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            //((IChangeProxyTarget)invocation).ChangeInvocationTarget(original);
            //invocation.Proceed();
            
            invocation.ReturnValue =
                invocation.Method.ToDelegate(typeof(T)).Invoke(original, invocation.Arguments);
        }
    }
}