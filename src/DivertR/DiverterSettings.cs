using DivertR.Default;
using DivertR.DispatchProxy;
using DivertR.Internal;

namespace DivertR
{
    public class DiverterSettings
    {
        private static DiverterSettings Settings = new DiverterSettings();
        private static readonly object SettingsLock = new object();
        
        public IProxyFactory ProxyFactory { get; }
        public IDefaultRootFactory DefaultRootFactory { get; }

        
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
            IDefaultRootFactory? defaultRootFactory = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
            DefaultRootFactory = defaultRootFactory ?? new DefaultRootFactory(new DefaultValueFactory(), this);
        }
    }
}