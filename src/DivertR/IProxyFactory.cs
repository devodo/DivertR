using System.Diagnostics.CodeAnalysis;

namespace DivertR
{
    public interface IProxyFactory
    {
        [return: NotNull]
        TTarget CreateProxy<TTarget>(IProxyCall<TTarget> proxyCall, TTarget? root = null) where TTarget : class?;
        
        void ValidateProxyTarget<TTarget>();
    }
}