namespace DivertR.Internal
{
    internal class DecoratorFactory : IDecoratorFactory
    {
        public object? CreateDecorator(IRedirect redirect, object? service)
        {
            if (service == null)
            {
                return null;
            }

            return redirect.Proxy(service);
        }
    }
}