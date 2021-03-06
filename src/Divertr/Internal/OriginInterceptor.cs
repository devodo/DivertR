using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal class OriginInterceptor<T> : IInterceptor where T : class
    {
        private readonly Relay<T> _relay;

        public OriginInterceptor(Relay<T> relay)
        {
            _relay = relay;
        }

        public void Intercept(IInvocation invocation)
        {
            var original = _relay.Current.Original;
            
            if (original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((IChangeProxyTarget)invocation).ChangeInvocationTarget(original);
            invocation.Proceed();
            
            //invocation.ReturnValue =
            //    invocation.Method.ToDelegate(typeof(T)).Invoke(redirectPipeline.Original, invocation.Arguments);
        }
    }
}