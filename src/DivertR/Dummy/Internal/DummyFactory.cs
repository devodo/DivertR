namespace DivertR.Dummy.Internal
{
    internal class DummyFactory : IDummyFactory
    {
        public TTarget Create<TTarget>(DiverterSettings diverterSettings) where TTarget : class
        {
            var via = new Via<TTarget>(diverterSettings, diverterSettings.DummyRedirectRepository);

            return via.Proxy(false);
        }
    }
}