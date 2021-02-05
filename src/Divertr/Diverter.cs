using System;
using Divertr.Internal;

namespace Divertr
{
    public class Diverter<T> : IDiverter<T> where T : class
    {
        private readonly DiverterId _diverterId;
        private readonly DiversionState _diversionState;
        private readonly Lazy<CallContext<T>> _callContext;

        public Diverter() : this(DiverterId.From<T>(), new DiversionState())
        {
        }
        
        internal Diverter(DiverterId diverterId, DiversionState diversionState)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }
            
            _diverterId = diverterId;
            _diversionState = diversionState;
            _callContext = new Lazy<CallContext<T>>(() => new CallContext<T>());
        }

        public ICallContext<T> CallCtx => _callContext.Value;
        
        public T Proxy(T? origin = null)
        {
            Diversion<T>? GetDiversion()
            {
                return _diversionState.GetDiversion<T>(_diverterId);
            }

            return ProxyFactory.Instance.CreateInstanceProxy(origin, GetDiversion);
        }
        
        public object Proxy(object? origin = null)
        {
            if (origin != null && !(origin is T))
            {
                throw new ArgumentException($"Not assignable to {typeof(T).Name}", nameof(origin));
            }

            Diversion<T>? GetDiversion()
            {
                return _diversionState.GetDiversion<T>(_diverterId);
            }

            return ProxyFactory.Instance.CreateInstanceProxy(origin as T, GetDiversion);
        }
        
        public IDiverter<T> Redirect(T target)
        {
            var diversion = new Diversion<T>(new Redirect<T>(target), _callContext.Value);
            _diversionState.SetDiversion(_diverterId, diversion);

            return this;
        }

        public IDiverter<T> AddRedirect(T target)
        {
            Diversion<T> Create()
            {
                return new Diversion<T>(new Redirect<T>(target), _callContext.Value);
            }

            Diversion<T> Update(Diversion<T> existingDiversion)
            {
                return existingDiversion.AppendRedirection(new Redirect<T>(target));
            }

            _diversionState.AddOrUpdateDiversion(_diverterId, Create, Update);

            return this;
        }

        public IDiverter<T> Reset()
        {
            _diversionState.Reset(_diverterId);

            return this;
        }
    }
}