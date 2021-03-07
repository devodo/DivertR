using System.Reflection;
using DivertR.Core;

namespace DivertR.DispatchProxy
{
    internal class DispatchProxyCall : ICall
    {
        public DispatchProxyCall(MethodInfo targetMethod, object[] args)
        {
            Method = targetMethod;
            Arguments = args;
        }

        public MethodInfo Method { get; }
        
        public object[] Arguments { get; }
    }
}