using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.DispatchProxy
{
    public class DiverterDispatchProxy : System.Reflection.DispatchProxy 
    {
        private IProxyInvoker _invoker = null!;

        public static TTarget Create<TTarget>(Func<TTarget, IProxyInvoker> invokerFactory) where TTarget : class?
        {
            object proxy = Create<TTarget, DiverterDispatchProxy>()!;
            ((DiverterDispatchProxy) proxy)._invoker = invokerFactory.Invoke((TTarget) proxy);

            return (TTarget) proxy;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Invoke(MethodInfo targetMethod, object[] args)
        {
            return _invoker.Invoke(targetMethod, args);
        }
    }
}
