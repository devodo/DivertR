using System.Reflection;
using Castle.DynamicProxy;
using DivertR.Core;

namespace DivertR.DynamicProxy
{
    internal class DynamicProxyCall : ICall
    {
        private readonly IInvocation _invocation;

        public DynamicProxyCall(IInvocation invocation)
        {
            _invocation = invocation;
        }

        public MethodInfo Method => _invocation.Method;
        public object[] Arguments => _invocation.Arguments;
    }
}