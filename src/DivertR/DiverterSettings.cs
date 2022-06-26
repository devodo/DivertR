using DivertR.DispatchProxy;
using DivertR.Dummy;
using DivertR.Dummy.Internal;
using DivertR.Internal;

namespace DivertR
{
    public class DiverterSettings
    {
        private static DiverterSettings GlobalSettings = new DiverterSettings();
        private static readonly object GlobalLock = new object();
        
        public IProxyFactory ProxyFactory { get; }

        public IDummyFactory DummyFactory { get; }
        
        public IRedirectRepository DummyRedirectRepository { get; }

        
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
            IRedirectRepository? dummyRedirectRepository = null,
            IDummyFactory? defaultRootFactory = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
            DummyRedirectRepository = dummyRedirectRepository ?? CreateDummyRepository();
            DummyFactory = defaultRootFactory ?? new DummyFactory();
        }

        private static IRedirectRepository CreateDummyRepository()
        {
            var redirect = new Redirect(new DummyCallHandler());
            
            return new RedirectRepository(new[] { redirect });
        }
    }
}