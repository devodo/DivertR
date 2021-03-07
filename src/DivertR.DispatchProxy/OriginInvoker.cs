using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DispatchProxy
{
    internal class OriginInvoker<T> : IDispatchProxyInvoker where T : class
    {
        private readonly IRelayState<T> _relayState;

        public OriginInvoker(IRelayState<T> relayState)
        {
            _relayState = relayState;
        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var original = _relayState.Original;
            
            if (original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }
            
            return targetMethod.ToDelegate(typeof(T)).Invoke(original, args);
        }
    }
}