using DivertR.DispatchProxy;
using DivertR.Dummy;
using DivertR.Dummy.Internal;

namespace DivertR
{
    public class DiverterSettings
    {
        private static DiverterSettings GlobalSettings = new DiverterSettings();
        private static readonly object GlobalLock = new object();
        
        public IProxyFactory ProxyFactory { get; }

        public IDummyFactory DummyFactory { get; }
        
        public IRedirect DummyRedirect { get; }

        
        public static DiverterSettings Global
        {
            get
            {
                lock (GlobalLock)
                {
                    return GlobalSettings;
                }
            }

            set
            {
                lock (GlobalLock)
                {
                    GlobalSettings = value;
                }
            }
        }

        public DiverterSettings(
            IProxyFactory? proxyFactory = null,
            IRedirect? dummyRedirect = null,
            IDummyFactory? defaultRootFactory = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
            DummyRedirect = dummyRedirect ?? new DummyRedirect();
            DummyFactory = defaultRootFactory ?? new DummyFactory();
        }
    }
}