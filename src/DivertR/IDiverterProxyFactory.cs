namespace DivertR
{
    public interface IDiverterProxyFactory
    {
        object? CreateProxy(IRedirect redirect, object? root);
    }
}