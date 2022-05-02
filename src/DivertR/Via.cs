using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DivertR.Internal;
using DivertR.Record;

namespace DivertR
{
    /// <inheritdoc />
    public class Via<TTarget> : IVia<TTarget> where TTarget : class
    {
        private readonly IProxyFactory _proxyFactory;
        private readonly Relay<TTarget> _relay;
        private readonly RedirectRepository<TTarget> _redirectRepository = new RedirectRepository<TTarget>();

        public Via(string? name = null) : this(ViaId.From<TTarget>(name), new ViaSet())
        {
            ((ViaSet) ViaSet).AddVia(this);
        }
        
        public Via(DiverterSettings? diverterSettings) : this(ViaId.From<TTarget>(), new ViaSet(diverterSettings))
        {
            ((ViaSet) ViaSet).AddVia(this);
        }
        
        public Via(string? name, DiverterSettings? diverterSettings) : this(ViaId.From<TTarget>(name), new ViaSet(diverterSettings))
        {
            ((ViaSet) ViaSet).AddVia(this);
        }
        
        internal Via(ViaId viaId, IViaSet viaSet)
        {
            _proxyFactory = viaSet.Settings.ProxyFactory;
            _proxyFactory.ValidateProxyTarget<TTarget>();
            _relay = new Relay<TTarget>(_proxyFactory);
            ViaId = viaId;
            ViaSet = viaSet;
        }
        
        public ViaId ViaId { get; }
        public IViaSet ViaSet { get; }
        public IRedirectPlan<TTarget> RedirectPlan => _redirectRepository.RedirectPlan;
        IRelay IVia.Relay => Relay;

        public IRelay<TTarget> Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _relay;
        }

        public TTarget Proxy(TTarget? root)
        {
            return _proxyFactory.CreateProxy(GetProxyCall, root);
        }
        
        public TTarget Proxy()
        {
            var defaultRoot = ViaSet.Settings.DummyFactory.Create<TTarget>(ViaSet.Settings);
            
            return _proxyFactory.CreateProxy(GetProxyCall, defaultRoot);
        }

        public object ProxyObject(object? root)
        {
            if (root != null && !(root is TTarget))
            {
                throw new ArgumentException($"Not assignable to {typeof(TTarget).Name}", nameof(root));
            }

            return Proxy(root as TTarget);
        }

        public object ProxyObject()
        {
            return Proxy();
        }

        IVia IVia.Reset()
        {
            return Reset();
        }

        IVia IVia.Strict(bool? isStrict)
        {
            return Strict(isStrict);
        }

        public IVia<TTarget> InsertRedirect(IRedirect<TTarget> redirect)
        {
            _redirectRepository.InsertRedirect(redirect);

            return this;
        }
        
        public IVia<TTarget> InsertRedirects(IEnumerable<IRedirect<TTarget>> redirects)
        {
            _redirectRepository.InsertRedirects(redirects);

            return this;
        }
        
        public IVia<TTarget> Reset()
        {
            _redirectRepository.Reset();

            return this;
        }
        
        public IVia<TTarget> Strict(bool? isStrict = true)
        {
            _redirectRepository.SetStrictMode(isStrict ?? true);

            return this;
        }
        
        public IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return To().Retarget(target, optionsAction);
        }
        
        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return To().Record(optionsAction);
        }

        public IRedirectBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            return new RedirectBuilder<TTarget>(this, callConstraint);
        }
        
        public IFuncRedirectBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression) where TReturn : struct
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var parsedCall = CallExpressionParser.FromExpression(constraintExpression.Body);
            
            return new FuncRedirectBuilder<TTarget, TReturn>(this, parsedCall);
        }

        public IClassFuncRedirectBuilder<TTarget, TReturn> To<TReturn>(Expression<IVia<TTarget>.ClassReturnMatch<TTarget, TReturn>> constraintExpression) where TReturn : class
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var parsedCall = CallExpressionParser.FromExpression(constraintExpression.Body);
            
            return new ClassFuncRedirectBuilder<TTarget, TReturn>(this, parsedCall);
        }

        public IActionRedirectBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var parsedCall = CallExpressionParser.FromExpression(constraintExpression.Body);
            
            return new ActionRedirectBuilder<TTarget>(this, parsedCall);
        }
        
        public IActionRedirectBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>> constraintExpression)
        {
            if (memberExpression.Body == null) throw new ArgumentNullException(nameof(memberExpression));
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            if (!(memberExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Must be a property member expression", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, constraintExpression.Body);

            return new ActionRedirectBuilder<TTarget>(this, parsedCall);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IProxyCall<TTarget>? GetProxyCall()
        {
            var redirectPlan = _redirectRepository.RedirectPlan;

            return redirectPlan == RedirectPlan<TTarget>.Empty
                ? null
                : new ViaProxyCall<TTarget>(_relay, redirectPlan);
        }
    }
}