using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DivertR.Internal;
using DivertR.Record;

namespace DivertR
{
    /// <inheritdoc />
    public class Via<TTarget> : IVia<TTarget> where TTarget : class?
    {
        private readonly IProxyFactory _proxyFactory;
        private readonly Relay<TTarget> _relay;
        private readonly ViaProxyCall<TTarget> _viaProxyCall;

        public Via(string? name = null, DiverterSettings? diverterSettings = null, IRedirectRepository? redirectRepository = null)
            : this(ViaId.From<TTarget>(name), new ViaSet(diverterSettings), redirectRepository)
        {
            ((ViaSet) ViaSet).AddVia(this);
        }
        
        public Via(DiverterSettings diverterSettings, IRedirectRepository? redirectRepository = null)
            : this(name: null, diverterSettings: diverterSettings, redirectRepository: redirectRepository)
        {
        }
        
        public Via(IRedirectRepository redirectRepository)
            : this(name: null, diverterSettings: null, redirectRepository: redirectRepository)
        {
        }

        internal Via(ViaId viaId, IViaSet viaSet, IRedirectRepository? redirectRepository = null)
        {
            _proxyFactory = viaSet.Settings.ProxyFactory;
            _proxyFactory.ValidateProxyTarget<TTarget>();
            _relay = new Relay<TTarget>(_proxyFactory, viaSet.Settings.CallInvoker);
            ViaId = viaId;
            ViaSet = viaSet;
            RedirectRepository = redirectRepository ?? new RedirectRepository();
            _viaProxyCall = new ViaProxyCall<TTarget>(_relay, RedirectRepository);
        }
        
        public ViaId ViaId { get; }
        public IViaSet ViaSet { get; }

        IRelay IVia.Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Relay;
        }

        public IRedirectRepository RedirectRepository
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public IRelay<TTarget> Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _relay;
        }
        
        object IVia.Proxy(object? root)
        {
            return Proxy(root);
        }

        object IVia.Proxy(bool withDummyRoot)
        {
            return Proxy(withDummyRoot);
        }

        object IVia.Proxy()
        {
            return Proxy();
        }

        [return: NotNull]
        public TTarget Proxy(object? root)
        {
            if (root != null && !(root is TTarget))
            {
                throw new ArgumentException($"Not assignable to {typeof(TTarget).Name}", nameof(root));
            }

            return Proxy(root as TTarget);
        }
        
        [return: NotNull]
        public TTarget Proxy(TTarget? root)
        {
            return _proxyFactory.CreateProxy(_viaProxyCall, root);
        }
        
        [return: NotNull]
        public TTarget Proxy(bool withDummyRoot)
        {
            var defaultRoot = withDummyRoot ? ViaSet.Settings.DummyFactory.Create<TTarget>(ViaSet.Settings) : null;
            
            return Proxy(defaultRoot);
        }
        
        [return: NotNull]
        public TTarget Proxy()
        {
            return Proxy(ViaSet.Settings.DefaultWithDummyRoot);
        }
        
        public IVia Redirect(IRedirect redirect, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var options = RedirectOptionsBuilder.Create(optionsAction);
            RedirectRepository.InsertRedirect(redirect, options);

            return this;
        }

        IVia IVia.Reset()
        {
            return Reset();
        }

        IVia IVia.Strict(bool? isStrict)
        {
            return Strict(isStrict);
        }

        public IVia<TTarget> Reset()
        {
            RedirectRepository.Reset();

            return this;
        }
        
        public IVia<TTarget> Strict(bool? isStrict = true)
        {
            RedirectRepository.SetStrictMode(isStrict ?? true);

            return this;
        }
        
        public IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return To().Retarget(target, optionsAction);
        }
        
        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return To().Record(optionsAction);
        }

        public IViaBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            return new ViaBuilder<TTarget>(this, RedirectBuilder<TTarget>.To(callConstraint));
        }

        public IFuncViaBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression)
        {
            return new FuncViaBuilder<TTarget, TReturn>(this, RedirectBuilder<TTarget>.To(constraintExpression));
        }

        public IActionViaBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            return new ActionViaBuilder<TTarget>(this, RedirectBuilder<TTarget>.To(constraintExpression));
        }
        
        public IActionViaBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null)
        {
            return new ActionViaBuilder<TTarget>(this, RedirectBuilder<TTarget>.ToSet(memberExpression, constraintExpression));
        }
    }

    public static class Via
    {
        private static readonly ProxyViaMap ProxyViaMap = new();
        
        /// <summary>
        /// Creates a Via proxy instance.
        /// </summary>
        /// <param name="root">The root instance the proxy will wrap and relay calls to by default.</param>
        /// <typeparam name="TTarget">The proxy target type.</typeparam>
        /// <returns>The proxy instance.</returns>
        public static TTarget Proxy<TTarget>(TTarget? root) where TTarget : class?
        {
            return Proxy<TTarget>(via => via.Proxy(root));
        }
        
        /// <summary>
        /// Creates a Via proxy instance with no given root instance.
        /// </summary>
        /// <param name="withDummyRoot">Flag to specify if the proxy should be created with either a dummy or a null root.</param>
        /// <typeparam name="TTarget">The proxy target type.</typeparam>
        /// <returns>The proxy instance.</returns>
        public static TTarget Proxy<TTarget>(bool withDummyRoot) where TTarget : class?
        {
            return Proxy<TTarget>(via => via.Proxy(withDummyRoot));
        }
        
        /// <summary>
        /// Creates a Via proxy instance with no given root instance.
        /// By default the proxy is created with a dummy root with members that return default values.
        /// The default behaviour can be changed to create with null root by setting the <see cref="DiverterSettings.DefaultWithDummyRoot" /> boolean flag.
        /// </summary>
        /// <typeparam name="TTarget">The proxy target type.</typeparam>
        /// <returns>The proxy instance.</returns>
        public static TTarget Proxy<TTarget>() where TTarget : class?
        {
            return Proxy<TTarget>(via => via.Proxy());
        }
        
        /// <summary>
        /// Retrieves the proxy instance's Via that controls its behaviour.
        /// </summary>
        /// <param name="proxy">The Via proxy instance.</param>
        /// <typeparam name="TTarget">The proxy and Via target type.</typeparam>
        /// <returns>The Via instance.</returns>
        public static IVia<TTarget> From<TTarget>(TTarget proxy) where TTarget : class
        {
            return ProxyViaMap.GetVia(proxy);
        }
        
        private static TTarget Proxy<TTarget>(Func<Via<TTarget>, TTarget> createProxy) where TTarget : class?
        {
            var via = new Via<TTarget>();
            var proxy = createProxy(via);
            ProxyViaMap.AddVia(proxy!, via);

            return proxy;
        }
    }
}