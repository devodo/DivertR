using System.Reflection;

namespace DivertR.Internal.DispatchProxy
{
    public interface IDispatchProxyInvoker
    {
        object Invoke(MethodInfo targetMethod, object[] args);
    }
}