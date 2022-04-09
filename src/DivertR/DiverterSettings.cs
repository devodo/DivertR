using DivertR.Default;
using DivertR.Default.Internal;
using DivertR.DispatchProxy;

namespace DivertR
{
    public class DiverterSettings
    {
        private static DiverterSettings Settings = new DiverterSettings();
        private static readonly object SettingsLock = new object();
        
        public IProxyFactory ProxyFactory { get; }

        public IDummyFactory DummyFactory { get; }

        
        public static DiverterSettings Global
        {
            get
            {
                lock (SettingsLock)
                {
                    return Settings;
                }
            }

            set
            {
                lock (SettingsLock)
                {
                    Settings = value;
                }
            }
        }

        public DiverterSettings(
            IProxyFactory? proxyFactory = null,
            IDefaultValueFactory? dummyFactory = null,
            IDummyFactory? defaultRootFactory = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
            dummyFactory ??= new DefaultValueFactory();
            DummyFactory = defaultRootFactory ?? new DummyFactory(dummyFactory, this);
        }
    }
}