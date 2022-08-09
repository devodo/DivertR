using System.Reflection;

namespace DivertR
{
    public interface IProxyCall<in TTarget> where TTarget : class?
    {
        object? Call(TTarget proxy, TTarget? root, MethodInfo method, CallArguments arguments);
    }
}