using System;
using System.Reflection;
using Castle.DynamicProxy;
using DivertR.Core;

namespace DivertR.DynamicProxy
{
    internal class DynamicProxyCall : ICall
    {
        private readonly IInvocation _invocation;
        private readonly Lazy<ParameterInfo[]> _parameters;

        public DynamicProxyCall(IInvocation invocation)
        {
            _invocation = invocation;
            _parameters = new Lazy<ParameterInfo[]>(() => Method.GetParameters());
        }

        public MethodInfo Method => _invocation.Method;
        
        public object[] Arguments => _invocation.Arguments;

        public ParameterInfo[] Parameters => _parameters.Value;
    }
}