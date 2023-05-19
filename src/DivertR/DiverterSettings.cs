using DivertR.DispatchProxy;
using DivertR.Dummy;
using DivertR.Internal;

namespace DivertR
{
    /// <summary>
    /// Diverter settings configuration.
    /// </summary>
    public class DiverterSettings
    {
        private static readonly object GlobalLock = new();
        private static readonly ICallInvoker DefaultCallInvoker = new LambdaExpressionCallInvoker();
        private static DiverterSettings GlobalSettings = new();
        
        /// <summary>
        /// The proxy factory that creates <see cref="IRedirect"/> proxies.
        /// </summary>
        public IProxyFactory ProxyFactory { get; }
        
        /// <summary>
        /// The factory used by Diverter to create service decorators from <see cref="IRedirect"/> proxies.
        /// </summary>
        public IDecoratorFactory DecoratorFactory { get; }
        
        /// <summary>
        /// The factory used to create proxy dummy roots.
        /// </summary>
        public IDummyFactory DummyFactory { get; }
        
        /// <summary>
        /// The call invoker used to forward calls to proxy targets.
        /// </summary>
        public ICallInvoker CallInvoker { get; }
        
        /// <summary>
        /// Enables proxy caching on root instances.
        /// When enabled <see cref="IRedirect"/> proxy creation will use cached proxies keyed by root reference. 
        /// </summary>
        public bool CacheRedirectProxies { get; }
        
        /// <summary>
        /// Global default settings.
        /// </summary>
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
        
        /// <summary>
        /// DiverterSettings constructor.
        /// </summary>
        /// <param name="proxyFactory">The proxy factory.</param>
        /// <param name="decoratorFactory">The decorator factory.</param>
        /// <param name="dummyFactory">The dummy root factory.</param>
        /// <param name="callInvoker">The proxy target call invoker.</param>
        /// <param name="cacheRedirectProxies">Enables proxy caching.</param>
        public DiverterSettings(IProxyFactory? proxyFactory = null,
            IDecoratorFactory? decoratorFactory = null,
            IDummyFactory? dummyFactory = null,
            ICallInvoker? callInvoker = null,
            bool cacheRedirectProxies = true)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
            DecoratorFactory = decoratorFactory ?? new DecoratorFactory();
            DummyFactory = dummyFactory ?? new DummyFactory();
            CallInvoker = callInvoker ?? DefaultCallInvoker;
            CacheRedirectProxies = cacheRedirectProxies;
        }
    }
}