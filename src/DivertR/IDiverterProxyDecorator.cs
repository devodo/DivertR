namespace DivertR
{
    public interface IDiverterProxyDecorator
    {
        object? Decorate(IVia via, object? original);
    }
}