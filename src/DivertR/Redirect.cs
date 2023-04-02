using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DivertR.Internal;
using DivertR.Record;

namespace DivertR
{
    /// <inheritdoc />
    public class Redirect<TTarget> : IRedirect<TTarget> where TTarget : class?
    {
        private readonly IProxyFactory _proxyFactory;
        private readonly Relay<TTarget> _relay;
        private readonly RedirectProxyCall<TTarget> _redirectProxyCall;
        private readonly ConditionalWeakTable<TTarget, TTarget> _proxyCache = new();

        public Redirect(string? name = null, DiverterSettings? diverterSettings = null, IRedirectRepository? redirectRepository = null)
            : this(RedirectId.From<TTarget>(name), new RedirectSet(diverterSettings), redirectRepository)
        {
            ((RedirectSet) RedirectSet).AddRedirect(this);
        }
        
        public Redirect(DiverterSettings? diverterSettings, IRedirectRepository? redirectRepository = null)
            : this(name: null, diverterSettings: diverterSettings, redirectRepository: redirectRepository)
        {
        }
        
        public Redirect(IRedirectRepository? redirectRepository)
            : this(name: null, diverterSettings: null, redirectRepository: redirectRepository)
        {
        }

        internal Redirect(RedirectId redirectId, IRedirectSet redirectSet, IRedirectRepository? redirectRepository = null)
        {
            _proxyFactory = redirectSet.Settings.ProxyFactory;
            _proxyFactory.ValidateProxyTarget<TTarget>();
            _relay = new Relay<TTarget>(_proxyFactory, redirectSet.Settings.CallInvoker);
            RedirectId = redirectId;
            RedirectSet = redirectSet;
            RedirectRepository = redirectRepository ?? new RedirectRepository();
            _redirectProxyCall = new RedirectProxyCall<TTarget>(_relay, RedirectRepository);
        }
        
        /// <inheritdoc />
        public RedirectId RedirectId { get; }
        
        /// <inheritdoc />
        public IRedirectSet RedirectSet { get; }
        
        /// <inheritdoc />
        IRelay IRedirect.Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Relay;
        }
        
        /// <inheritdoc />
        public IRedirectRepository RedirectRepository
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        /// <inheritdoc />
        public IRelay<TTarget> Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _relay;
        }
        
        /// <inheritdoc />
        object IRedirect.Proxy(object? root)
        {
            return Proxy(root);
        }
        
        /// <inheritdoc />
        object IRedirect.Proxy(bool withDummyRoot)
        {
            return Proxy(withDummyRoot);
        }

        /// <inheritdoc />
        object IRedirect.Proxy()
        {
            return Proxy();
        }
        
        /// <inheritdoc />
        [return: NotNull]
        public TTarget Proxy(object? root)
        {
            if (root != null && root is not TTarget)
            {
                throw new ArgumentException($"Not assignable to {typeof(TTarget).Name}", nameof(root));
            }

            return Proxy(root as TTarget);
        }
        
        /// <inheritdoc />
        [return: NotNull]
        public TTarget Proxy(TTarget? root)
        {
            TTarget proxy;
            
            if (root is null || !RedirectSet.Settings.CacheRedirectProxies)
            {
                proxy = _proxyFactory.CreateProxy(_redirectProxyCall, root);
                RedirectSet.Settings.RedirectProxyDecorator.Decorate(this, proxy);
            }
            else
            {
                proxy = _proxyCache.GetValue(root, x =>
                {
                    var createdProxy = _proxyFactory.CreateProxy(_redirectProxyCall, x);
                    RedirectSet.Settings.RedirectProxyDecorator.Decorate(this, createdProxy);

                    return createdProxy;
                });
            }

            return proxy!;
        }
        
        /// <inheritdoc />
        [return: NotNull]
        public TTarget Proxy(bool withDummyRoot)
        {
            var defaultRoot = withDummyRoot ? RedirectSet.Settings.DummyFactory.Create<TTarget>(RedirectSet.Settings) : null;
            
            return Proxy(defaultRoot);
        }
        
        /// <inheritdoc />
        [return: NotNull]
        public TTarget Proxy()
        {
            return Proxy(RedirectSet.Settings.DefaultWithDummyRoot);
        }
        
        /// <inheritdoc />
        public IRedirect<TTarget> Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var options = ViaOptionsBuilder.Create(optionsAction);
            RedirectRepository.InsertVia(via, options);

            return this;
        }
        
        /// <inheritdoc />
        IRedirect IRedirect.Via(IVia via, Action<IViaOptionsBuilder>? optionsAction)
        {
            return Via(via, optionsAction);
        }
        
        /// <inheritdoc />
        IRedirect IRedirect.Reset()
        {
            return Reset();
        }
        
        /// <inheritdoc />
        IRedirect IRedirect.Strict(bool? isStrict)
        {
            return Strict(isStrict);
        }
        
        /// <inheritdoc />
        public IRedirect<TTarget> Reset()
        {
            ResetInternal();

            return this;
        }
        
        /// <inheritdoc />
        public IRedirect<TTarget> Strict(bool? isStrict = true)
        {
            RedirectRepository.SetStrictMode(isStrict ?? true);

            return this;
        }
        
        /// <inheritdoc />
        public IRedirect<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            To().Retarget(target, optionsAction);

            return this;
        }
        
        /// <inheritdoc />
        public IRecordStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            return To().Record(optionsAction);
        }
        
        /// <inheritdoc />
        public IRedirectUpdater<TTarget> To(ICallConstraint? callConstraint = null)
        {
            return new RedirectUpdater<TTarget>(this, ViaBuilder<TTarget>.ToInternal(callConstraint));
        }
        
        /// <inheritdoc />
        public IFuncRedirectUpdater<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression)
        {
            return new FuncRedirectUpdater<TTarget, TReturn>(this, ViaBuilder<TTarget>.ToInternal(constraintExpression));
        }
        
        /// <inheritdoc />
        public IActionRedirectUpdater<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            return new ActionRedirectUpdater<TTarget>(this, ViaBuilder<TTarget>.ToInternal(constraintExpression));
        }
        
        /// <inheritdoc />
        public IActionRedirectUpdater<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null)
        {
            return new ActionRedirectUpdater<TTarget>(this, ViaBuilder<TTarget>.ToSetInternal(memberExpression, constraintExpression));
        }
        
        protected virtual void ResetInternal()
        {
            RedirectRepository.Reset();
        }
    }
}
