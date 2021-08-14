using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DivertR.Core;
using DivertR.Internal;
using DivertR.Record;
using DivertR.Record.Internal;
using DivertR.Setup;

namespace DivertR
{
    public class Via<TTarget> : IVia<TTarget> where TTarget : class
    {
        private readonly RedirectRepository _redirectRepository;
        private readonly IProxyFactory _proxyFactory;
        private readonly RelayContext<TTarget> _relayContext;

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
            Relay = new Relay<TTarget>(_relayContext, _proxyFactory);
        }

        public ViaId ViaId { get; }

        public IRelay<TTarget> Relay { get; }

        public TTarget Next => Relay.Next;

        public IReadOnlyList<Redirect<TTarget>> ConfiguredRedirects =>
            _redirectRepository.Get<TTarget>(ViaId)?.RedirectItems ?? Array.Empty<Redirect<TTarget>>();

        public TTarget Proxy(TTarget? original = null)
        {
            IProxyCall<TTarget>? GetProxyCall()
            {
                var redirectState = _redirectRepository.Get<TTarget>(ViaId);

                return redirectState == null
                    ? null
                    : new ViaProxyCall<TTarget>(_relayContext, redirectState);
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

        public IVia<TTarget> InsertRedirect(Redirect<TTarget> redirect)
        {
            _redirectRepository.InsertRedirect(ViaId, redirect);

            return this;
        }
        
        public IVia<TTarget> Reset()
        {
            _redirectRepository.Reset(ViaId);

            return this;
        }
        
        public IVia<TTarget> Retarget(TTarget target)
        {
            return To().Retarget(target);
        }

        public IRedirectBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            return new RedirectBuilder<TTarget>(this, callConstraint);
        }
        
        public IFuncRedirectBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new FuncRedirectBuilder<TTarget, TReturn>(this, parsedCall);
        }
        
        public IActionRedirectBuilder<TTarget> To(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return new ActionRedirectBuilder<TTarget>(this, parsedCall);
        }
        
        public IActionRedirectBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
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
        
        public ICallStream<TTarget> Record(ICallConstraint<TTarget>? callConstraint = null)
        {
            var recordRedirect = new RecordCallHandler<TTarget>(Relay);
            var redirect = new Redirect<TTarget>(recordRedirect, callConstraint, int.MaxValue, true);
            InsertRedirect(redirect);

            return recordRedirect.CreateCallStream();
        }
    }
}