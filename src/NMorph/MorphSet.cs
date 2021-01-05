using System;
using Castle.DynamicProxy;

namespace NMorph
{
    public class MorphSet
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
        
        public void Substitute<T>(T substitute, string groupName = null) where T : class
        {
            _alterationStore.AddAlteration<T>(groupName, _ => substitute);
        }

        public void Substitute<T>(Func<IInvocationContext<T>, T> getSubstitute, string groupName = null) where T : class
        {
            _alterationStore.AddAlteration(groupName, getSubstitute);
        }
        
        public bool Reset<T>()
        {
            return _alterationStore.Reset<T>();
        }

        public void Reset()
        {
            _alterationStore.Reset();
        }
    }
}
