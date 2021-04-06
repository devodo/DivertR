using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DivertR.Core;
using DivertR.DispatchProxy;
using DivertR.Internal;
using DivertR.Setup;

namespace DivertR
{
    public class Via<TTarget> : IVia<TTarget> where TTarget : class
    {
        private readonly RedirectRepository _redirectRepository;
        private readonly IProxyFactory _proxyFactory;
        private readonly RelayContext<TTarget> _relayContext;
        private readonly Lazy<IRelay<TTarget>> _relay;

        public Via(IDiverterSettings? diverterSettings = null) : this(ViaId.From<TTarget>(), new RedirectRepository(), diverterSettings ?? DiverterSettings.Default)
        {
        }
        
        internal Via(ViaId viaId, RedirectRepository redirectRepository, IDiverterSettings diverterSettings)
        {
            _proxyFactory = diverterSettings.ProxyFactory;
            _proxyFactory.ValidateProxyTarget<TTarget>();

            ViaId = viaId;
            _redirectRepository = redirectRepository;
            
            _relayContext = new RelayContext<TTarget>();
            _relay = new Lazy<IRelay<TTarget>>(() => new Relay<TTarget>(_relayContext, _proxyFactory));
        }

        public ViaId ViaId { get; }

        public IRelay<TTarget> Relay => _relay.Value;
        public TTarget Next => Relay.Next;
        
        public IReadOnlyList<IRedirect<TTarget>> Redirects => _redirectRepository.Get<TTarget>(ViaId);

        public TTarget Proxy(TTarget? original = null)
        {
            IProxyCall<TTarget>? GetProxyCall()
            {
                var redirects = _redirectRepository.Get<TTarget>(ViaId);

                return redirects.Length == 0
                    ? null
                    : new ViaProxyCall<TTarget>(_relayContext, redirects);
            }

            return _proxyFactory.CreateProxy(original, GetProxyCall);
        }

        public object ProxyObject(object? original = null)
        {
            if (original != null && !(original is TTarget))
            {
                throw new ArgumentException($"Not assignable to {typeof(TTarget).Name}", nameof(original));
            }

            return Proxy(original as TTarget);
        }

        public IVia<TTarget> InsertRedirect(IRedirect<TTarget> redirect, int orderWeight = 0)
        {
            _redirectRepository.InsertRedirect(ViaId, redirect, orderWeight);

            return this;
        }
        
        public IVia<TTarget> Reset()
        {
            _redirectRepository.Reset(ViaId);

            return this;
        }
        
        public IVia<TTarget> RedirectTo(TTarget target)
        {
            return Redirect().To(target);
        }

        public IRedirectBuilder<TTarget> Redirect(ICallConstraint<TTarget>? callConstraint = null)
        {
            return new RedirectBuilder<TTarget>(this, callConstraint);
        }
        
        public IFuncRedirectBuilder<TTarget, TReturn> Redirect<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new FuncRedirectBuilder<TTarget, TReturn>(this, parsedCall);
        }
        
        public IActionRedirectBuilder<TTarget> Redirect(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new ActionRedirectBuilder<TTarget>(this, parsedCall);
        }
        
        public IActionRedirectBuilder<TTarget> RedirectSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));
            if (valueExpression?.Body == null) throw new ArgumentNullException(nameof(valueExpression));

            if (!(lambdaExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Only property member expressions are valid input to RedirectSet", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, valueExpression.Body);

            return new ActionRedirectBuilder<TTarget>(this, parsedCall);
        }

        public ICallRecord<TTarget> RecordCalls(ICallConstraint<TTarget>? callConstraint = null, int orderWeight = int.MinValue)
        {
            var callCapture = new CallRecordRedirect<TTarget>(Relay);
            InsertRedirect(callCapture, orderWeight);

            return callCapture;
        }
    }
}