using System;
using Divertr.Internal;

namespace Divertr
{
    public class Diversion<T> : IDiversion<T> where T : class
    {
        private readonly DiversionId _diversionId;
        private readonly RouteRepository _routeRepository;
        private readonly Lazy<CallContext<T>> _callContext;

        public Diversion() : this(DiversionId.From<T>(), new RouteRepository())
        {
        }
        
        internal Diversion(DiversionId diversionId, RouteRepository routeRepository)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }
            
            _diversionId = diversionId;
            _routeRepository = routeRepository;
            _callContext = new Lazy<CallContext<T>>(() => new CallContext<T>());
        }

        public ICallContext<T> CallCtx => _callContext.Value;
        
        public T Proxy(T? root = null)
        {
            DiversionRoute<T>? GetDiversionRoute()
            {
                return _routeRepository.GetRoute<T>(_diversionId);
            }

            return ProxyFactory.Instance.CreateDiversionProxy(root, GetDiversionRoute);
        }
        
        public object Proxy(object? root = null)
        {
            if (root != null && !(root is T))
            {
                throw new ArgumentException($"Not assignable to {typeof(T).Name}", nameof(root));
            }

            return Proxy(root as T);
        }
        
        public IDiversion<T> Redirect(T target)
        {
            var redirect = new Redirect<T>(target);
            var callRoute = new DiversionRoute<T>(redirect, _callContext.Value);
            _routeRepository.SetRoute(_diversionId, callRoute);

            return this;
        }

        public IDiversion<T> AddRedirect(T target)
        {
            DiversionRoute<T> Create()
            {
                return new DiversionRoute<T>(new Redirect<T>(target), _callContext.Value);
            }

            DiversionRoute<T> Update(DiversionRoute<T> existing)
            {
                return existing.AppendRedirect(new Redirect<T>(target));
            }

            _routeRepository.AddOrUpdateRoute(_diversionId, Create, Update);

            return this;
        }

        public IDiversion<T> Reset()
        {
            _routeRepository.Reset(_diversionId);

            return this;
        }
    }
}