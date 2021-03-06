using System.Reflection;

namespace DivertR.Internal.DispatchProxy
{
    public class DiverterProxy : System.Reflection.DispatchProxy 
    {
        private IDispatchProxyInvoker _invoker = null!;

        public static T Create<T>(IDispatchProxyInvoker invoker) where T : class
        {
            object proxy = Create<T, DiverterProxy>()!;
            ((DiverterProxy)proxy)._invoker = invoker;

            return (T)proxy;
        }
        
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _invoker.Invoke(targetMethod, args);
        }
    }
}