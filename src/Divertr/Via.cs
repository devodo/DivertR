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

                return redirects.IsEmpty
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
        
        public IVia<T> AddRedirect(IRedirect<T> redirect)
        {
            _redirectRepository.AddRedirect(ViaId, redirect);

            return this;
        }

        public IVia<T> InsertRedirect(int index, IRedirect<T> redirect)
        {
            _redirectRepository.InsertRedirect(ViaId, index, redirect);

            return this;
        }

        public IVia<T> RemoveRedirect(IRedirect<T> redirect)
        {
            _redirectRepository.RemoveRedirect(ViaId, redirect);

            return this;
        }

        public IVia<T> RemoveRedirectAt(int index)
        {
            _redirectRepository.RemoveRedirectAt<T>(ViaId, index);

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

        public IRedirectBuilder<T> Redirect(ICallConstraint<T>? callConstraint = null)
        {
            return new RedirectBuilder<T>(this, callConstraint);
        }
        
        public IFuncRedirectBuilder<T, TReturn> Redirect<TReturn>(Expression<Func<T, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new FuncRedirectBuilder<T, TReturn>(this, parsedCall);
        }
        
        public IActionRedirectBuilder<T> Redirect(Expression<Action<T>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new ActionRedirectBuilder<T>(this, parsedCall);
        }
        
        public IActionRedirectBuilder<T> RedirectSet<TProperty>(Expression<Func<T, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));
            if (valueExpression?.Body == null) throw new ArgumentNullException(nameof(valueExpression));

            if (!(lambdaExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Only property member expressions are valid input to RedirectSet", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, valueExpression.Body);

            return new ActionRedirectBuilder<T>(this, parsedCall);
        }

        public ICallRecord<T> RecordCalls(ICallConstraint<T>? callConstraint = null)
        {
            return InsertRecordCalls(0, callConstraint);
        }

        public ICallRecord<T> AddRecordCalls(ICallConstraint<T>? callConstraint = null)
        {
            var callCapture = new CallRecordRedirect<T>(Relay);
            AddRedirect(callCapture);

            return callCapture;
        }

        public ICallRecord<T> InsertRecordCalls(int index, ICallConstraint<T>? callConstraint = null)
        {
            var callCapture = new CallRecordRedirect<T>(Relay);
            InsertRedirect(index, callCapture);

            return callCapture;
        }
    }
}