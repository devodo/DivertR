namespace DivertR.Internal
{
    internal class DependencyFactory : IDependencyFactory
    {
        public object? Create(IVia via, object? original)
        {
            if (original == null)
            {
                return null;
            }

            return via.Proxy(original);
        }
    }
}