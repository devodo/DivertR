using DivertR.DispatchProxy;
using DivertR.Dummy;
using DivertR.Internal;

namespace DivertR
{
    public class DiverterSettings
    {
        private static readonly object GlobalLock = new();
        private static readonly ICallInvoker DefaultCallInvoker = new LambdaExpressionCallInvoker();
        private static DiverterSettings GlobalSettings = new();

        public IProxyFactory ProxyFactory { get; }
        
        public IDiverterProxyFactory DiverterProxyFactory { get; }
        
        public IRedirectProxyDecorator RedirectProxyDecorator { get; }
        
        public bool DefaultWithDummyRoot { get; }

        public IDummyFactory DummyFactory { get; }
        
        public ICallInvoker CallInvoker { get; }

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
            IDiverterProxyFactory? diverterProxyFactory = null,
            IRedirectProxyDecorator? redirectProxyDecorator = null,
            bool defaultWithDummyRoot = true,
            IDummyFactory? dummyFactory = null,
            ICallInvoker? callInvoker = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
            DiverterProxyFactory = diverterProxyFactory ?? new DiverterProxyFactory();
            RedirectProxyDecorator = redirectProxyDecorator ?? new RedirectProxyDecorator();
            DefaultWithDummyRoot = defaultWithDummyRoot;
            DummyFactory = dummyFactory ?? new DummyFactory();
            CallInvoker = callInvoker ?? DefaultCallInvoker;
        }
    }
}