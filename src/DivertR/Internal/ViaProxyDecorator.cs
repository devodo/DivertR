namespace DivertR.Internal
{
    internal class ViaProxyDecorator : IViaProxyDecorator
    {
        public object? Decorate(IVia via, object? original)
        {
            if (original == null)
            {
                return null;
            }

            return via.Proxy(original);
        }
    }
}