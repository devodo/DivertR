using System;
using Divertr.Internal;

namespace Divertr
{
    public class Router<T> : IRouter<T> where T : class
    {
        private readonly RouteRepository _routeRepository;
        private readonly Lazy<CallRelay<T>> _callContext;

        public Router() : this(RouterId.From<T>(), new RouteRepository())
        {
        }
        
        internal Router(RouterId routerId, RouteRepository routeRepository)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }
            
            RouterId = routerId;
            _routeRepository = routeRepository;
            _callContext = new Lazy<CallRelay<T>>(() => new CallRelay<T>());
        }

        public RouterId RouterId { get; }

        public ICallRelay<T> Relay => _callContext.Value;
        
        public T Proxy(T? original = null)
        {
            RedirectRoute<T>? GetRedirectRoute()
            {
                return _routeRepository.GetRoute<T>(RouterId);
            }

            return ProxyFactory.Instance.CreateDiverterProxy(original, GetRedirectRoute);
        }

        public object Proxy(object? original = null)
        {
            if (original != null && !(original is T))
            {
                throw new ArgumentException($"Not assignable to {typeof(T).Name}", nameof(original));
            }

            return Proxy(original as T);
        }
        
        public IRouter<T> Redirect(T target, object? state = null)
        {
            var redirect = new Redirect<T>(target, state);
            var callRoute = new RedirectRoute<T>(redirect, _callContext.Value);
            _routeRepository.SetRoute(RouterId, callRoute);

            return this;
        }

        public IRouter<T> AddRedirect(T target, object? state = null)
        {
            RedirectRoute<T> Create()
            {
                return new RedirectRoute<T>(new Redirect<T>(target, state), _callContext.Value);
            }

            RedirectRoute<T> Update(RedirectRoute<T> existing)
            {
                return existing.AppendRedirect(new Redirect<T>(target, state));
            }

            _routeRepository.AddOrUpdateRoute(RouterId, Create, Update);

            return this;
        }

        public IRouter<T> Reset()
        {
            _routeRepository.Reset(RouterId);

            return this;
        }
    }
}