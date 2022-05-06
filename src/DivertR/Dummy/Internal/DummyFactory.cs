namespace DivertR.Dummy.Internal
{
    internal class DummyFactory : IDummyFactory
    {
        public TTarget Create<TTarget>(DiverterSettings diverterSettings) where TTarget : class
        {
            var via = new Via<TTarget>(diverterSettings);
            var redirect = new WrappedRedirect<TTarget>(diverterSettings.DummyRedirect);
            via.InsertRedirect(redirect);

            return via.Proxy(null);
        }
    }
}