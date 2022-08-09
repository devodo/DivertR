using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DivertR
{
    public interface IProxyCall<in TTarget> where TTarget : class?
    {
        object? Call([DisallowNull] TTarget proxy, TTarget? root, MethodInfo method, CallArguments arguments);
    }
}