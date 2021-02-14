using System;
using Divertr.Internal;

namespace Divertr
{
    public class Diversion<T> : IDiversion<T> where T : class
    {
        private readonly DiversionId _diversionId;
        private readonly DiverterState _diverterState;
        private readonly Lazy<CallContext<T>> _callContext;

        public Diversion() : this(DiversionId.From<T>(), new DiverterState())
        {
        }
        
        internal Diversion(DiversionId diversionId, DiverterState diverterState)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }
            
            _diversionId = diversionId;
            _diverterState = diverterState;
            _callContext = new Lazy<CallContext<T>>(() => new CallContext<T>());
        }

        public ICallContext<T> CallCtx => _callContext.Value;
        
        public T Proxy(T? original = null)
        {
            DiversionSnapshot<T>? GetDirector()
            {
                return _diverterState.GetCallRoute<T>(_diversionId);
            }

            return ProxyFactory.Instance.CreateDiversionProxy(original, GetDirector);
        }
        
        public object Proxy(object? origin = null)
        {
            if (origin != null && !(origin is T))
            {
                throw new ArgumentException($"Not assignable to {typeof(T).Name}", nameof(origin));
            }

            return Proxy((T) origin!);
        }
        
        public IDiversion<T> Redirect(T target)
        {
            var redirect = new Redirect<T>(target);
            var callRoute = new DiversionSnapshot<T>(redirect, _callContext.Value);
            _diverterState.SetCallRoute(_diversionId, callRoute);

            return this;
        }

        public IDiversion<T> AddRedirect(T target)
        {
            DiversionSnapshot<T> Create()
            {
                return new DiversionSnapshot<T>(new Redirect<T>(target), _callContext.Value);
            }

            DiversionSnapshot<T> Update(DiversionSnapshot<T> existing)
            {
                return existing.AppendRedirect(new Redirect<T>(target));
            }

            _diverterState.AddOrUpdateCallRoute(_diversionId, Create, Update);

            return this;
        }

        public IDiversion<T> Reset()
        {
            _diverterState.Reset(_diversionId);

            return this;
        }
    }
}