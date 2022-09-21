namespace DivertR
{
    public interface IViaProxyDecorator
    {
        object? Decorate(IVia via, object? original);
    }
}