using System;

namespace NMorph
{
    public class Morph
    {
        private readonly AlterationStore _alterationStore = new AlterationStore();

        public T Create<T>(T origin = null) where T : class
        {
            return Create(default, origin);
        }

        public T Create<T>(string groupName, T origin = null) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            Alteration<T> GetAlteration()
            {
                return _alterationStore.GetAlteration<T>(groupName);
            }
            
            return ProxyFactory.Instance.CreateMorphProxy(origin, GetAlteration);
        }
        
        public IAlterationBuilder<T> Intercept<T>(string groupName = null) where T : class
        {
            return new AlterationBuilder<T>(_alterationStore, groupName);
        }
        
        public IAlterationBuilder<T> Intercept<T>(out ICallContext<T> callContext, string groupName = null) where T : class
        {
            callContext = _alterationStore.GetOrAddAlteration<T>(groupName).CallContext;
            return new AlterationBuilder<T>(_alterationStore, groupName);
        }

        public void Reset()
        {
            _alterationStore.Reset();
        }
    }
}