using System.Reflection;

namespace DivertR.Internal.DispatchProxy
{
    internal class RedirectInvoker<T> : IDispatchProxyInvoker where T : class
    {
        private readonly Relay<T> _relay;

        public RedirectInvoker(Relay<T> relay)
        {
            _relay = relay;
        }
        
        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var call = new DispatchProxyCall(targetMethod, args);
            var redirect = _relay.BeginNextRedirect(call);
            
            if (redirect == null)
            {
                var original = _relay.Current.Original;
                if (original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }
                
                return targetMethod.ToDelegate(typeof(T)).Invoke(original, args);
            }

            try
            {

                if (redirect.Target == null)
                {
                    throw new DiverterException("The redirect instance reference is null");
                }
                
                return targetMethod.ToDelegate(typeof(T)).Invoke(redirect.Target, args);
            }
            finally
            {
                _relay.EndRedirect(call);
            }
        }
    }
}