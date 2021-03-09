using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DispatchProxy
{
    internal class RedirectInvoker<T> : IDispatchProxyInvoker where T : class
    {
        private readonly IRelayState<T> _relayState;

        public RedirectInvoker(IRelayState<T> relayState)
        {
            _relayState = relayState;
        }
        
        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var call = new DispatchProxyCall(targetMethod, args);
            var redirect = _relayState.BeginNextRedirect(call);
            
            if (redirect == null)
            {
                var original = _relayState.Original;
                if (original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }
                
                return targetMethod.ToDelegate(typeof(T)).Invoke(original, args);
            }

            try
            {
                return redirect.Invoke(targetMethod, args)!;
            }
            finally
            {
                _relayState.EndRedirect(call);
            }
        }
    }
}