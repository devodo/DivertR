using System.Reflection;

namespace DivertR.DispatchProxy
{
    public interface IProxyInvoker
    {
        object? Invoke(MethodInfo targetMethod, object[] args);
    }
}