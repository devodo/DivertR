using System;
using System.Reflection;

namespace DivertR.DispatchProxy
{
    internal class DiverterDispatchProxy : System.Reflection.DispatchProxy 
    {
        private IProxyInvoker _invoker = null!;

        public static T Create<T>(IProxyInvoker invoker) where T : class
        {
            return Create<T>(proxy => invoker);
        }

        public static T Create<T>(Func<T, IProxyInvoker> invokerFactory) where T : class
        {
            object proxy = Create<T, DiverterDispatchProxy>()!;
            ((DiverterDispatchProxy) proxy)._invoker = invokerFactory.Invoke((T) proxy);

            return (T) proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _invoker.Invoke(targetMethod, args);
        }
    }
}