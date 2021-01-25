using System;

namespace NMorph
{
    public class Divertr
    {
        private readonly DiversionStore _diversionStore = new DiversionStore();

        public T Proxy<T>(T origin = null) where T : class
        {
            return Proxy(default, origin);
        }

        public T Proxy<T>(string groupName, T origin = null) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            Diversion<T> GetDiversion()
            {
                return _diversionStore.GetDiversion<T>(groupName);
            }
            
            return ProxyFactory.Instance.CreateInstanceProxy(origin, GetDiversion);
        }
        
        public IDiversionBuilder<T> Redirect<T>(string groupName = null) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }
            
            return new DiversionBuilder<T>(_diversionStore, groupName);
        }

        public IDiversionBuilder<T> Redirect<T>(Action<IDiversionBuilder<T>> callback, string groupName = null) where T : class
        {
            var alterationBuilder = new DiversionBuilder<T>(_diversionStore, groupName);
            callback.Invoke(alterationBuilder);

            return alterationBuilder;
        }

        public ICallContext<T> CallContext<T>(string groupName = null) where T : class
        {
            return _diversionStore.GetOrAddAlteration<T>(groupName).CallContext;
        }

        public void Reset()
        {
            _diversionStore.Reset();
        }
    }
}