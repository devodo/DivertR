using System.Reflection;

namespace DivertR.DispatchProxy
{
    internal interface IProxyInvoker
    {
        object Invoke(MethodInfo targetMethod, object[] args);
    }
}