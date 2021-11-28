using DivertR.DispatchProxy;

namespace DivertR.Setup
{
    public class DiverterSettings
    {
        private static DiverterSettings Settings = new DiverterSettings();
        private static readonly object SettingsLock = new object();

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

        public DiverterSettings(IProxyFactory? proxyFactory = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
        }

        public IProxyFactory ProxyFactory { get; }
    }
}