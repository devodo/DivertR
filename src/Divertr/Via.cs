using System;
using System.Linq;
using System.Linq.Expressions;
using DivertR.Core;
using DivertR.Core.Internal;
using DivertR.DispatchProxy;
using DivertR.Internal;

namespace DivertR
{
    public class Via<T> : IVia<T> where T : class
    {
        private readonly ViaStateRepository _viaStateRepository;
        private readonly IProxyFactory _proxyFactory;
        private readonly RelayState<T> _relayState;
        private readonly Lazy<IRelay<T>> _relay;

        public Via() : this(ViaId.From<T>(), new ViaStateRepository())
        {
        }
        
        internal Via(ViaId viaId, ViaStateRepository viaStateRepository)
        {
            if (!typeof(T).IsInterface && !typeof(T).IsClass)
            {
                throw new ArgumentException("Only interface and types are supported", typeof(T).Name);
            }
            
            ViaId = viaId;
            _viaStateRepository = viaStateRepository;
            _proxyFactory = DispatchProxyFactory.Instance;
            _relayState = new RelayState<T>();
            _relay = new Lazy<IRelay<T>>(() => new Relay<T>(_relayState, _proxyFactory));
        }

        public ViaId ViaId { get; }

        public IRelay<T> Relay => _relay.Value;
        
        public T Proxy(T? original = null)
        {
            ViaState<T>? GetRedirectRoute()
            {
                return _viaStateRepository.Get<T>(ViaId);
            }

            return _proxyFactory.CreateDiverterProxy(original, GetRedirectRoute);
        }

        public object ProxyObject(object? original = null)
        {
            if (original != null && !(original is T))
            {
                throw new ArgumentException($"Not assignable to {typeof(T).Name}", nameof(original));
            }

            return Proxy(original as T);
        }
        
        public IVia<T> RedirectTo(T target, object? state = null)
        {
            var redirect = new TargetRedirect<T>(target, state);
            
            return AddRedirect(redirect);
        }

        public IRedirectBuilder<T> Redirect(ICallCondition? callCondition = null)
        {
            return new RedirectBuilder<T>(this, callCondition);
        }
        
        public IRedirectBuilder<T, TResult> Redirect<TResult>(Expression<Func<T, TResult>> expression)
        {
            return new RedirectBuilder<T, TResult>(this, expression);
        }

        public IVia<T> AddRedirect(IRedirect<T> redirect)
        {
            ViaState<T> Create()
            {
                return new ViaState<T>(redirect, _relayState);
            }

            ViaState<T> Update(ViaState<T> existing)
            {
                var redirects = existing.Redirects.ToList();
                redirects.Add(redirect);
                return new ViaState<T>(redirects, _relayState);
            }

            _viaStateRepository.AddOrUpdate(ViaId, Create, Update);

            return this;
        }

        public IVia<T> InsertRedirect(int index, T target, object? state = null)
        {
            var redirect = new TargetRedirect<T>(target, state);
            
            ViaState<T> Create()
            {
                if (index != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0 if there are no existing redirects");
                }
                
                return new ViaState<T>(redirect, _relayState);
            }

            ViaState<T> Update(ViaState<T> existing)
            {
                if (index < 0 || index > existing.Redirects.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the existing redirects");
                }
                
                var redirects = existing.Redirects.ToList();
                redirects.Insert(index, redirect);
                return new ViaState<T>(redirects, _relayState);
            }

            _viaStateRepository.AddOrUpdate(ViaId, Create, Update);

            return this;
        }

        public IVia<T> Reset()
        {
            _viaStateRepository.Reset(ViaId);

            return this;
        }
    }
}