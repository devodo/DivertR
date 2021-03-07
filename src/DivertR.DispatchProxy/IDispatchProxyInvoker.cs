using System.Reflection;

namespace DivertR.DispatchProxy
{
    internal interface IDispatchProxyInvoker
    {
        object Invoke(MethodInfo targetMethod, object[] args);
    }
}