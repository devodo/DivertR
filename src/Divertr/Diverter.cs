using System;
using Divertr.Internal;

namespace Divertr
{
    public class Diverter<T> : IDiverter<T> where T : class
    {
        private readonly DiverterId _diverterId;
        private readonly DiverterState _diverterState;
        private readonly Lazy<CallContext<T>> _callContext;

        public Diverter() : this(DiverterId.From<T>(), new DiverterState())
        {
        }
        
        internal Diverter(DiverterId diverterId, DiverterState diverterState)
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
            Director<T>? GetDirector()
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
        
        public IDiverter<T> Redirect(T target)
        {
            var diversion = new Director<T>(new Redirect<T>(target), _callContext.Value);
            _diverterState.SetDirector(_diverterId, diversion);

            return this;
        }

        public IDiverter<T> AddRedirect(T target)
        {
            Director<T> Create()
            {
                return new Director<T>(new Redirect<T>(target), _callContext.Value);
            }

            Director<T> Update(Director<T> existingDiversion)
            {
                return existingDiversion.AppendRedirect(new Redirect<T>(target));
            }

            _diverterState.AddOrUpdateDirector(_diverterId, Create, Update);

            return this;
        }

        public IDiverter<T> Reset()
        {
            _diverterState.Reset(_diverterId);

            return this;
        }
    }
}