using System;
using Divertr.Internal;

namespace Divertr
{
    public class Director<T> : IDirector<T> where T : class
    {
        private readonly DiverterId _diverterId;
        private readonly DiverterState _diverterState;
        private readonly Lazy<CallContext<T>> _callContext;

        public Director() : this(DiverterId.From<T>(), new DiverterState())
        {
        }
        
        internal Director(DiverterId diverterId, DiverterState diverterState)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }
            
            _diverterId = diverterId;
            _diverterState = diverterState;
            _callContext = new Lazy<CallContext<T>>(() => new CallContext<T>());
        }

        public ICallContext<T> CallCtx => _callContext.Value;
        
        public T Proxy(T? original = null)
        {
            CallRoute<T>? GetDirector()
            {
                return _diverterState.GetDirector<T>(_diverterId);
            }

            return ProxyFactory.Instance.CreateInstanceProxy(original, GetDirector);
        }
        
        public object Proxy(object? origin = null)
        {
            if (origin != null && !(origin is T))
            {
                throw new ArgumentException($"Not assignable to {typeof(T).Name}", nameof(origin));
            }

            return Proxy((T) origin!);
        }
        
        public IDirector<T> Redirect(T target)
        {
            var director = new CallRoute<T>(new Redirect<T>(target), _callContext.Value);
            _diverterState.SetDirector(_diverterId, director);

            return this;
        }

        public IDirector<T> AddRedirect(T target)
        {
            CallRoute<T> Create()
            {
                return new CallRoute<T>(new Redirect<T>(target), _callContext.Value);
            }

            CallRoute<T> Update(CallRoute<T> existingDiversion)
            {
                return existingDiversion.AppendRedirect(new Redirect<T>(target));
            }

            _diverterState.AddOrUpdateDirector(_diverterId, Create, Update);

            return this;
        }

        public IDirector<T> Reset()
        {
            _diverterState.Reset(_diverterId);

            return this;
        }
    }
}