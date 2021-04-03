using System;
using System.Reflection;

namespace DivertR.DispatchProxy
{
    internal class DiverterDispatchProxy : System.Reflection.DispatchProxy 
    {
        private IProxyInvoker _invoker = null!;

        public static TTarget Create<TTarget>(IProxyInvoker invoker) where TTarget : class
        {
            return Create<TTarget>(proxy => invoker);
        }

        public static TTarget Create<TTarget>(Func<TTarget, IProxyInvoker> invokerFactory) where TTarget : class
        {
            object proxy = Create<TTarget, DiverterDispatchProxy>()!;
            ((DiverterDispatchProxy) proxy)._invoker = invokerFactory.Invoke((TTarget) proxy);

            return (TTarget) proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _invoker.Invoke(targetMethod, args);
        }
    }
}