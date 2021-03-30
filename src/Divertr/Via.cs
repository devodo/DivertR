using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DivertR.Core;
using DivertR.DispatchProxy;
using DivertR.Internal;

namespace DivertR
{
    public class Via<T> : IVia<T> where T : class
    {
        private readonly RedirectRepository _redirectRepository;
        private readonly IProxyFactory _proxyFactory;
        private readonly RelayContext<T> _relayContext;
        private readonly Lazy<IRelay<T>> _relay;

        public Via() : this(ViaId.From<T>(), new RedirectRepository(), DispatchProxyFactory.Instance)
        {
        }
        
        internal Via(ViaId viaId, RedirectRepository redirectRepository, IProxyFactory proxyFactory)
        {
            proxyFactory.Validate<T>();

            ViaId = viaId;
            _redirectRepository = redirectRepository;
            _proxyFactory = proxyFactory;
            _relayContext = new RelayContext<T>();
            _relay = new Lazy<IRelay<T>>(() => new Relay<T>(_relayContext, _proxyFactory));
        }

        public ViaId ViaId { get; }

        public IRelay<T> Relay => _relay.Value;
        public T Next => Relay.Next;

        public T Proxy(T? original = null)
        {
            IProxyCall<T>? GetProxyCall()
            {
                var redirects = _redirectRepository.Get<T>(ViaId);

                return redirects.Length == 0
                    ? null
                    : new ViaProxyCall<T>(_relayContext, redirects);
            }

            return _proxyFactory.CreateProxy(original, GetProxyCall);
        }

        public object ProxyObject(object? original = null)
        {
            if (original != null && !(original is T))
            {
                throw new ArgumentException($"Not assignable to {typeof(T).Name}", nameof(original));
            }

            return Proxy(original as T);
        }

        public IReadOnlyList<IRedirect<T>> Redirects => _redirectRepository.Get<T>(ViaId);
        
        public IVia<T> InsertRedirect(IRedirect<T> redirect, int orderWeight = 0)
        {
            _redirectRepository.InsertRedirect(ViaId, redirect, orderWeight);

            return this;
        }
        
        public IVia<T> Reset()
        {
            _redirectRepository.Reset(ViaId);

            return this;
        }
        
        public IVia<T> RedirectTo(T target)
        {
            return Redirect().To(target);
        }

        public IDelegateRedirectBuilder<T> Redirect(ICallConstraint<T>? callConstraint = null)
        {
            return new DelegateRedirectBuilder<T>(this, callConstraint);
        }
        
        public IFuncRedirectBuilder<T, TReturn> Redirect<TReturn>(Expression<Func<T, TReturn>> lambdaExpression)
        {
            return new RedirectBuilder<T>(this).When(lambdaExpression);
        }
        
        public IActionRedirectBuilder<T> Redirect(Expression<Action<T>> lambdaExpression)
        {
            return new RedirectBuilder<T>(this).When(lambdaExpression);
        }
        
        public IActionRedirectBuilder<T> RedirectSet<TProperty>(Expression<Func<T, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            return new RedirectBuilder<T>(this).WhenSet(lambdaExpression, valueExpression);
        }

        public ICallRecord<T> RecordCalls(ICallConstraint<T>? callConstraint = null, int orderWeight = int.MinValue)
        {
            var callCapture = new CallRecordRedirect<T>(Relay);
            InsertRedirect(callCapture, orderWeight);

            return callCapture;
        }
    }
}