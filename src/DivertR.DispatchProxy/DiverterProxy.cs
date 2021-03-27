using System;
using System.Reflection;

namespace DivertR.DispatchProxy
{
    internal class DiverterProxy : System.Reflection.DispatchProxy 
    {
        private IDispatchProxyInvoker _invoker = null!;

        public static T Create<T>(IDispatchProxyInvoker invoker) where T : class
        {
            return Create<T>(proxy => invoker);
        }

        public static T Create<T>(Func<T, IDispatchProxyInvoker> invokerFactory) where T : class
        {
            object proxy = Create<T, DiverterProxy>()!;
            ((DiverterProxy) proxy)._invoker = invokerFactory.Invoke((T) proxy);

            return (T) proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _invoker.Invoke(targetMethod, args);
        }
    }
}