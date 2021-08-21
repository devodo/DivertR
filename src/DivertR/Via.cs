using System;
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
        private readonly Relay<TTarget> _relay;

        public Via(IDiverterSettings? diverterSettings = null) : this(ViaId.From<TTarget>(), new RedirectRepository(), diverterSettings ?? DiverterSettings.Default)
        {
        }
        
        internal Via(ViaId viaId, RedirectRepository redirectRepository, IDiverterSettings diverterSettings)
        {
            _proxyFactory = diverterSettings.ProxyFactory;
            _proxyFactory.ValidateProxyTarget<TTarget>();
            ViaId = viaId;
            _redirectRepository = redirectRepository;
            _relay = new Relay<TTarget>(_proxyFactory);
        }

        public ViaId ViaId { get; }

        public IRelay<TTarget> Relay => _relay;

        public RedirectPlan<TTarget> RedirectPlan =>
            _redirectRepository.Get<TTarget>(ViaId) ?? RedirectPlan<TTarget>.Empty;

        public TTarget Proxy(TTarget? original = null)
        {
            IProxyCall<TTarget>? GetProxyCall()
            {
                var redirectPlan = _redirectRepository.Get<TTarget>(ViaId);

                return redirectPlan == null
                    ? null
                    : new ViaProxyCall<TTarget>(_relay, redirectPlan);
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
                throw new ArgumentException("Must be a property member expression", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, valueExpression.Body);

            return new ActionRedirectBuilder<TTarget>(this, parsedCall);
        }

        public IVia<TTarget> Strict()
        {
            _redirectRepository.SetStrictMode<TTarget>(ViaId);

            return this;
        }
        
        IVia IVia.Strict()
        {
            return Strict();
        }
        
        public ICallStream<TTarget> Record(ICallConstraint<TTarget>? callConstraint = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>(Relay);
            var redirect = new Redirect<TTarget>(recordHandler, callConstraint, int.MaxValue, true);
            InsertRedirect(redirect);

            return recordHandler.CallStream;
        }
    }
}