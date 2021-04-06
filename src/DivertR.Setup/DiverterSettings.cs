using System.Collections.Concurrent;
using DivertR.Core;
using DivertR.DispatchProxy;

namespace DivertR.Setup
{
    public class DiverterSettings : IDiverterSettings
    {
        private readonly ConcurrentDictionary<string, object> _settings = new ConcurrentDictionary<string, object>();

        public static readonly DiverterSettings Default = new DiverterSettings();

        public DiverterSettings()
        {
            ProxyFactory = new DispatchProxyFactory();
        }

        public IProxyFactory ProxyFactory
        {
            get => (IProxyFactory) _settings[nameof(ProxyFactory)];
            set => _settings[nameof(ProxyFactory)] = value;
        }
    }
}