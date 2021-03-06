using Castle.DynamicProxy;

namespace DivertR.Internal.DynamicProxy
{
    public class DynamicProxyCall : ICall
    {
        private readonly IInvocation _invocation;

        public DynamicProxyCall(IInvocation invocation)
        {
            _invocation = invocation;
        }
    }
}