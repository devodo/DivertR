using System;
using System.Linq;
using DivertR.Internal;

namespace DivertR
{
    public class Via<T> : IVia<T> where T : class
    {
        private readonly ViaWayRepository _viaWayRepository;
        private readonly Lazy<Relay<T>> _callRelay;

        public Via() : this(ViaId.From<T>(), new ViaWayRepository())
        {
        }
        
        internal Via(ViaId viaId, ViaWayRepository viaWayRepository)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }
            
            ViaId = viaId;
            _viaWayRepository = viaWayRepository;
            _callRelay = new Lazy<Relay<T>>(() => new Relay<T>());
        }

        public ViaId ViaId { get; }

        public IRelay<T> Relay => _callRelay.Value;
        
        public T Proxy(T? original = null)
        {
            ViaWay<T>? GetRedirectRoute()
            {
                return _viaWayRepository.Get<T>(ViaId);
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
        
        public IVia<T> Redirect(T target, object? state = null)
        {
            var redirect = new Redirect<T>(target, state);
            var callRoute = new ViaWay<T>(redirect, _callRelay.Value);
            _viaWayRepository.Set(ViaId, callRoute);

            return this;
        }

        public IVia<T> AddRedirect(T target, object? state = null)
        {
            var redirect = new Redirect<T>(target, state);
            
            ViaWay<T> Create()
            {
                return new ViaWay<T>(redirect, _callRelay.Value);
            }

            ViaWay<T> Update(ViaWay<T> existing)
            {
                var redirects = existing.Redirects.ToList();
                redirects.Add(redirect);
                return new ViaWay<T>(redirects, _callRelay.Value);
            }

            _viaWayRepository.AddOrUpdate(ViaId, Create, Update);

            return this;
        }

        public IVia<T> InsertRedirect(int index, T target, object? state = null)
        {
            var redirect = new Redirect<T>(target, state);
            
            ViaWay<T> Create()
            {
                if (index != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0 if there are no existing redirects");
                }
                
                return new ViaWay<T>(redirect, _callRelay.Value);
            }

            ViaWay<T> Update(ViaWay<T> existing)
            {
                if (index < 0 || index > existing.Redirects.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the existing redirects");
                }
                
                var redirects = existing.Redirects.ToList();
                redirects.Insert(index, redirect);
                return new ViaWay<T>(redirects, _callRelay.Value);
            }

            _viaWayRepository.AddOrUpdate(ViaId, Create, Update);

            return this;
        }

        public IVia<T> Reset()
        {
            _viaWayRepository.Reset(ViaId);

            return this;
        }
    }
}