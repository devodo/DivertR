using DivertR.DispatchProxy;
using DivertR.Dummy;

namespace DivertR
{
    public class DiverterSettings
    {
        private static DiverterSettings GlobalSettings = new DiverterSettings();
        private static readonly object GlobalLock = new object();
        
        public IProxyFactory ProxyFactory { get; }

        public bool DefaultWithDummyRoot { get; }

        public IDummyFactory DummyFactory { get; }


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
            bool defaultWithDummyRoot = true,
            IDummyFactory? dummyFactory = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
            DefaultWithDummyRoot = defaultWithDummyRoot;
            DummyFactory = dummyFactory ?? new DummyFactory();
        }
    }
}