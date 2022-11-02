namespace DivertR.Internal
{
    internal class DiverterProxyDecorator : IDiverterProxyDecorator
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