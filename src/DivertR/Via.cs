using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DivertR.Internal;
using DivertR.Record;

namespace DivertR
{
    /// <inheritdoc />
    public abstract class Via : IVia
    {
        private readonly RedirectRepository _redirectRepository = new RedirectRepository();
        private readonly Relay _relay;
        
        private protected Via(ViaId viaId, IViaSet viaSet, Relay relay)
        {
            ViaId = viaId;
            ViaSet = viaSet;
            _relay = relay;
        }
        
        public ViaId ViaId { get; }
        public IViaSet ViaSet { get; }
        public IRelay Relay => _relay;
        public IRedirectPlan RedirectPlan => _redirectRepository.RedirectPlan;

        public abstract object ProxyObject(object? root);
        public abstract object ProxyObject();

        public IVia InsertRedirect(Redirect redirect)
        {
            _redirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IVia InsertRedirects(IEnumerable<Redirect> redirects)
        {
            _redirectRepository.InsertRedirects(redirects);

            return this;
        }

        public IVia Reset()
        {
            _redirectRepository.Reset();

            return this;
        }

        public IVia Strict(bool? isStrict)
        {
            _redirectRepository.SetStrictMode(isStrict ?? true);

            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private protected IProxyCall? GetProxyCall()
        {
            var redirectPlan = _redirectRepository.RedirectPlan;

            return redirectPlan == DivertR.Internal.RedirectPlan.Empty
                ? null
                : new ViaProxyCall(_relay, redirectPlan);
        }
    }

    /// <inheritdoc cref="DivertR.IVia{TTarget}" />
    public class Via<TTarget> : Via, IVia<TTarget> where TTarget : class
    {
        private readonly IProxyFactory _proxyFactory;

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
        
        internal Via(ViaId viaId, IViaSet viaSet) : base(viaId, viaSet, new Relay<TTarget>(viaSet.Settings.ProxyFactory))
        {
            _proxyFactory = viaSet.Settings.ProxyFactory;
            _proxyFactory.ValidateProxyTarget<TTarget>();
        }
        
        public new IRelay<TTarget> Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (IRelay<TTarget>) base.Relay;
        }

        public TTarget Proxy(TTarget? root)
        {
            return _proxyFactory.CreateProxy(GetProxyCall, root);
        }
        
        public TTarget Proxy()
        {
            var defaultRoot = ViaSet.Settings.DummyFactory.Create<TTarget>();
            
            return _proxyFactory.CreateProxy(GetProxyCall, defaultRoot);
        }

        public override object ProxyObject(object? root)
        {
            if (root != null && !(root is TTarget))
            {
                throw new ArgumentException($"Not assignable to {typeof(TTarget).Name}", nameof(root));
            }

            return Proxy(root as TTarget);
        }

        public override object ProxyObject()
        {
            return Proxy();
        }
        
        public new IVia<TTarget> InsertRedirect(Redirect redirect)
        {
            base.InsertRedirect(redirect);

            return this;
        }
        
        public new IVia<TTarget> InsertRedirects(IEnumerable<Redirect> redirects)
        {
            base.InsertRedirects(redirects);

            return this;
        }
        
        public new IVia<TTarget> Reset()
        {
            base.Reset();

            return this;
        }
        
        public new IVia<TTarget> Strict(bool? isStrict = true)
        {
            base.Strict(isStrict ?? true);

            return this;
        }
        
        public IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return To().Retarget(target, optionsAction);
        }
        
        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return To().Record(optionsAction);
        }

        public IRedirectBuilder<TTarget> To(ICallConstraint? callConstraint = null)
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
    }
}