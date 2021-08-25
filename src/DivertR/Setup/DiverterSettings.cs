using DivertR.DispatchProxy;

namespace DivertR.Setup
{
    public class DiverterSettings : IDiverterSettings
    {
        public static readonly DiverterSettings Default = new DiverterSettings();

        public DiverterSettings(IProxyFactory? proxyFactory = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
        }

        public IProxyFactory ProxyFactory { get; }
    }
}