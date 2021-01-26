using System;
using System.Collections.Concurrent;

namespace Divertr
{
    public class Diverter
    {
        private readonly DiversionStore _diversionStore = new DiversionStore();
        private readonly ConcurrentDictionary<DiversionId, object> _diverters = new ConcurrentDictionary<DiversionId, object>();

        public IDiverter<T> Of<T>(string groupName = null) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            return (IDiverter<T>) _diverters.GetOrAdd(DiversionId.From<T>(groupName),
                id => new Diverter<T>(id, _diversionStore, new CallContext<T>()));
        }

        public void Reset()
        {
            _diversionStore.Reset();
        }
    }

    public class Diverter<T> : IDiverter<T> where T : class
    {
        private readonly DiversionId _diversionId;
        private readonly DiversionStore _diversionStore;
        private readonly CallContext<T> _callContext;

        public Diverter(string groupName = null) : this(DiversionId.From<T>(groupName), new DiversionStore(), new CallContext<T>())
        {
        }

        internal Diverter(DiversionId diversionId, DiversionStore diversionStore, CallContext<T> callContext)
        {
            _diversionId = diversionId;
            _diversionStore = diversionStore;
            _callContext = callContext;
        }

        public ICallContext<T> CallContext => _callContext;

        public T Proxy(T origin = null)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            Diversion<T> GetDiversion()
            {
                return _diversionStore.GetDiversion<T>(_diversionId);
            }

            return ProxyFactory.Instance.CreateInstanceProxy(origin, GetDiversion);
        }

        public IDiverter<T> Redirect(T redirect)
        {
            var diversion = new Diversion<T>(new Redirection<T>(redirect), _callContext);
            _diversionStore.SetDiversion(_diversionId, diversion);

            return this;
        }

        public IDiverter<T> AddRedirect(T redirect)
        {
            Diversion<T> Create()
            {
                return new Diversion<T>(new Redirection<T>(redirect), _callContext);
            }

            Diversion<T> Update(Diversion<T> existingDiversion)
            {
                return existingDiversion.AppendRedirection(new Redirection<T>(redirect));
            }

            _diversionStore.AddOrUpdateDiversion(_diversionId, Create, Update);

            return this;
        }

        public IDiverter<T> Reset()
        {
            _diversionStore.Reset(_diversionId);

            return this;
        }
    }
}