namespace DivertR
{
    public interface IProxyFactory
    {
        TTarget CreateProxy<TTarget>(IProxyCall<TTarget> proxyCall, TTarget? root = null) where TTarget : class;
        void ValidateProxyTarget<TTarget>();
    }
}