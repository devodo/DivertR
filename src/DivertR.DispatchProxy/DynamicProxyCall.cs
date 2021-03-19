using System;
using System.Reflection;
using DivertR.Core;

namespace DivertR.DispatchProxy
{
    internal class DispatchProxyCall : ICall
    {
        private readonly Lazy<ParameterInfo[]> _parameters;
        
        public DispatchProxyCall(MethodInfo targetMethod, object[] args)
        {
            Method = targetMethod;
            Arguments = args;
            _parameters = new Lazy<ParameterInfo[]>(() => Method.GetParameters());
        }

        public MethodInfo Method { get; }

        public object[] Arguments { get; }

        public ParameterInfo[] Parameters => _parameters.Value;
    }
}