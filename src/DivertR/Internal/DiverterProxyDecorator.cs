namespace DivertR.Internal
{
    internal class DiverterProxyDecorator : IDiverterProxyDecorator
    {
        public object? Decorate(IRedirect redirect, object? original)
        {
            if (original == null)
            {
                return null;
            }

            return redirect.Proxy(original);
        }
    }
}