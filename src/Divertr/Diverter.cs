using System;
using Divertr.Internal;

namespace Divertr
{
    public class Diverter<T> : IDiverter<T> where T : class
    {
        private readonly RouteRepository _routeRepository;
        private readonly Lazy<CallContext<T>> _callContext;

        public Diverter() : this(DiverterId.From<T>(), new RouteRepository())
        {
        }
        
        internal Diverter(DiverterId diverterId, RouteRepository routeRepository)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }
            
            DiverterId = diverterId;
            _routeRepository = routeRepository;
            _callContext = new Lazy<CallContext<T>>(() => new CallContext<T>());
        }

        public DiverterId DiverterId { get; }

        public ICallContext<T> CallCtx => _callContext.Value;
        
        public T Proxy(T? root = null)
        {
            DiversionRoute<T>? GetDiversionRoute()
            {
                return _routeRepository.GetRoute<T>(DiverterId);
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
        
        public IDiverter<T> SendTo(T target)
        {
            var redirect = new Redirect<T>(target);
            var callRoute = new DiversionRoute<T>(redirect, _callContext.Value);
            _routeRepository.SetRoute(DiverterId, callRoute);

            return this;
        }

        public IDiverter<T> AddSendTo(T target)
        {
            DiversionRoute<T> Create()
            {
                return new DiversionRoute<T>(new Redirect<T>(target), _callContext.Value);
            }

            DiversionRoute<T> Update(DiversionRoute<T> existing)
            {
                return existing.AppendRedirect(new Redirect<T>(target));
            }

            _routeRepository.AddOrUpdateRoute(DiverterId, Create, Update);

            return this;
        }

        public IDiverter<T> Reset()
        {
            _routeRepository.Reset(DiverterId);

            return this;
        }
    }
}