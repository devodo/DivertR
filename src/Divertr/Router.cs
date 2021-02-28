using System;
using System.Linq;
using DivertR.Internal;

namespace DivertR
{
    public class Router<T> : IRouter<T> where T : class
    {
        private readonly RouteRepository _routeRepository;
        private readonly Lazy<CallRelay<T>> _callRelay;

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
            _callRelay = new Lazy<CallRelay<T>>(() => new CallRelay<T>());
        }

        public RouterId RouterId { get; }

        public ICallRelay<T> Relay => _callRelay.Value;
        
        public T Proxy(T? original = null)
        {
            RedirectRoute<T>? GetRedirectRoute()
            {
                return _routeRepository.GetRoute<T>(RouterId);
            }

            return ProxyFactory.Instance.CreateDiverterProxy(original, GetRedirectRoute);
        }

        public object ProxyObject(object? original = null)
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
            var callRoute = new RedirectRoute<T>(redirect, _callRelay.Value);
            _routeRepository.SetRoute(RouterId, callRoute);

            return this;
        }

        public IRouter<T> AddRedirect(T target, object? state = null)
        {
            var redirect = new Redirect<T>(target, state);
            
            RedirectRoute<T> Create()
            {
                return new RedirectRoute<T>(redirect, _callRelay.Value);
            }

            RedirectRoute<T> Update(RedirectRoute<T> existing)
            {
                var redirects = existing.Redirects.ToList();
                redirects.Add(redirect);
                return new RedirectRoute<T>(redirects, _callRelay.Value);
            }

            _routeRepository.AddOrUpdateRoute(RouterId, Create, Update);

            return this;
        }

        public IRouter<T> InsertRedirect(int index, T target, object? state = null)
        {
            var redirect = new Redirect<T>(target, state);
            
            RedirectRoute<T> Create()
            {
                if (index != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0 if there are no existing redirects");
                }
                
                return new RedirectRoute<T>(redirect, _callRelay.Value);
            }

            RedirectRoute<T> Update(RedirectRoute<T> existing)
            {
                if (index < 0 || index > existing.Redirects.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the existing redirects");
                }
                
                var redirects = existing.Redirects.ToList();
                redirects.Insert(index, redirect);
                return new RedirectRoute<T>(redirects, _callRelay.Value);
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