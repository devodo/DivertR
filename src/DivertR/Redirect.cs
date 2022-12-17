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

        public Redirect(string? name = null, DiverterSettings? diverterSettings = null, IRedirectRepository? redirectRepository = null)
            : this(RedirectId.From<TTarget>(name), new RedirectSet(diverterSettings), redirectRepository)
        {
            ((RedirectSet) RedirectSet).AddRedirect(this);
        }
        
        public Redirect(DiverterSettings diverterSettings, IRedirectRepository? redirectRepository = null)
            : this(name: null, diverterSettings: diverterSettings, redirectRepository: redirectRepository)
        {
        }
        
        public Redirect(IRedirectRepository redirectRepository)
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
            if (root != null && !(root is TTarget))
            {
                throw new ArgumentException($"Not assignable to {typeof(TTarget).Name}", nameof(root));
            }

            return Proxy(root as TTarget);
        }
        
        /// <inheritdoc />
        [return: NotNull]
        public TTarget Proxy(TTarget? root)
        {
            var proxy = _proxyFactory.CreateProxy(_redirectProxyCall, root);
            
            return RedirectSet.Settings.RedirectProxyDecorator.Decorate(this, proxy);
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
        public IRedirect<TTarget> Via(IVia redirect, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var options = ViaOptionsBuilder.Create(optionsAction);
            RedirectRepository.InsertVia(redirect, options);

            return this;
        }
        
        /// <inheritdoc />
        IRedirect IRedirect.Via(IVia redirect, Action<IViaOptionsBuilder>? optionsAction)
        {
            return Via(redirect, optionsAction);
        }
        
        /// <inheritdoc />
        IRedirect IRedirect.Reset(bool includePersistent)
        {
            return Reset(includePersistent);
        }
        
        /// <inheritdoc />
        IRedirect IRedirect.Strict(bool? isStrict)
        {
            return Strict(isStrict);
        }
        
        /// <inheritdoc />
        public IRedirect<TTarget> Reset(bool includePersistent = false)
        {
            RedirectRepository.Reset(includePersistent);

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
        public IRedirectUpdater<TTarget> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            return new RedirectUpdater<TTarget>(this, ViaBuilder<TTarget>.To(callConstraint));
        }
        
        /// <inheritdoc />
        public IFuncRedirectUpdater<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression)
        {
            return new FuncRedirectUpdater<TTarget, TReturn>(this, ViaBuilder<TTarget>.To(constraintExpression));
        }
        
        /// <inheritdoc />
        public IActionRedirectUpdater<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            return new ActionRedirectUpdater<TTarget>(this, ViaBuilder<TTarget>.To(constraintExpression));
        }
        
        /// <inheritdoc />
        public IActionRedirectUpdater<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null)
        {
            return new ActionRedirectUpdater<TTarget>(this, ViaBuilder<TTarget>.ToSet(memberExpression, constraintExpression));
        }
    }

    public static class Redirect
    {
        internal static readonly ProxyRedirectMap ProxyRedirectMap = new();
        
        /// <summary>
        /// Creates a Redirect proxy instance.
        /// </summary>
        /// <param name="root">The root instance the proxy will wrap and relay calls to by default.</param>
        /// <typeparam name="TTarget">The proxy target type.</typeparam>
        /// <returns>The proxy instance.</returns>
        public static TTarget Proxy<TTarget>(TTarget? root) where TTarget : class?
        {
            return Proxy<TTarget>(redirect => redirect.Proxy(root));
        }
        
        /// <summary>
        /// Creates a Redirect proxy instance with no given root instance.
        /// </summary>
        /// <param name="withDummyRoot">Flag to specify if the proxy should be created with either a dummy or a null root.</param>
        /// <typeparam name="TTarget">The proxy target type.</typeparam>
        /// <returns>The proxy instance.</returns>
        public static TTarget Proxy<TTarget>(bool withDummyRoot) where TTarget : class?
        {
            return Proxy<TTarget>(redirect => redirect.Proxy(withDummyRoot));
        }
        
        /// <summary>
        /// Creates a Redirect proxy instance with no given root instance.
        /// By default the proxy is created with a dummy root with members that return default values.
        /// The default behaviour can be changed to create with null root by setting the <see cref="DiverterSettings.DefaultWithDummyRoot" /> boolean flag.
        /// </summary>
        /// <typeparam name="TTarget">The proxy target type.</typeparam>
        /// <returns>The proxy instance.</returns>
        public static TTarget Proxy<TTarget>() where TTarget : class?
        {
            return Proxy<TTarget>(redirect => redirect.Proxy());
        }
        
        /// <summary>
        /// Retrieves the proxy instance's Redirect that controls its behaviour.
        /// </summary>
        /// <param name="proxy">The Redirect proxy instance.</param>
        /// <typeparam name="TTarget">The proxy and Redirect target type.</typeparam>
        /// <returns>The Redirect instance.</returns>
        public static IRedirect<TTarget> From<TTarget>(TTarget proxy) where TTarget : class
        {
            return ProxyRedirectMap.GetRedirect(proxy);
        }
        
        private static TTarget Proxy<TTarget>(Func<Redirect<TTarget>, TTarget> createProxy) where TTarget : class?
        {
            var redirect = new Redirect<TTarget>();
            
            return createProxy(redirect);
        }
    }
}