using System.Reflection;

namespace DivertR.Internal.DispatchProxy
{
    internal class OriginInvoker<T> : IDispatchProxyInvoker where T : class
    {
        private readonly Relay<T> _relay;

        public OriginInvoker(Relay<T> relay)
        {
            _relay = relay;
        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var original = _relay.Current.Original;
            
            if (original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }
            
            return targetMethod.ToDelegate(typeof(T)).Invoke(original, args);
        }
    }
}