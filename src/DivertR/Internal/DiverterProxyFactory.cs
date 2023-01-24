namespace DivertR.Internal
{
    internal class DiverterProxyFactory : IDiverterProxyFactory
    {
        public object? CreateProxy(IRedirect redirect, object? root)
        {
            if (root == null)
            {
                return null;
            }

            return redirect.Proxy(root);
        }
    }
}