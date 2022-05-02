using DivertR.Default;
using DivertR.DispatchProxy;
using DivertR.Dummy.Internal;

namespace DivertR
{
    public class DiverterSettings
    {
        private static DiverterSettings Settings = new DiverterSettings();
        private static readonly object SettingsLock = new object();
        
        public IProxyFactory ProxyFactory { get; }

        public IDummyFactory DummyFactory { get; }
        
        public IRedirect DummyRedirect { get; }

        
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
            IRedirect? dummyRedirect = null,
            IDummyFactory? defaultRootFactory = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
            DummyRedirect ??= new DummyRedirect();
            DummyFactory = defaultRootFactory ?? new DummyFactory();
        }
    }
}