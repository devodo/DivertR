using System;
using Castle.DynamicProxy;

namespace NMorph
{
    public class Morpher
    {
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        private readonly AlterationStore _alterationStore = new AlterationStore();

        public T CreateMorph<T>(T origin = null) where T : class
        {
            return CreateMorph(default, origin);
        }

        public T CreateMorph<T>(string groupName, T origin = null) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            var interceptor = new MorphInterceptor<T>(_alterationStore, origin, groupName);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(origin, interceptor);
        }

        public void Substitute<T>(Func<IMorphSource<T>, T> getSubstitute, string groupName = null) where T : class
        {
            _alterationStore.UpdateAlteration(groupName, getSubstitute, true);
        }

        public void Reset()
        {
            _alterationStore.Reset();
        }
    }
}
