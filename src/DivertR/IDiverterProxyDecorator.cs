namespace DivertR
{
    public interface IDiverterProxyDecorator
    {
        object? Decorate(IRedirect redirect, object? original);
    }
}