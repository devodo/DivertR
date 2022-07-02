using System;
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
            _relay = new Relay<TTarget>(_proxyFactory);
            ViaId = viaId;
            ViaSet = viaSet;
            RedirectRepository = redirectRepository ?? new RedirectRepository();
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
        
        public TTarget Proxy(object? root)
        {
            if (root != null && !(root is TTarget))
            {
                throw new ArgumentException($"Not assignable to {typeof(TTarget).Name}", nameof(root));
            }

            return Proxy(root as TTarget);
        }

        public TTarget Proxy(TTarget? root)
        {
            return _proxyFactory.CreateProxy(GetProxyCall, root);
        }

        public TTarget Proxy(bool withDummyRoot)
        {
            var defaultRoot = withDummyRoot ? ViaSet.Settings.DummyFactory.Create<TTarget>(ViaSet.Settings) : null;
            
            return _proxyFactory.CreateProxy(GetProxyCall, defaultRoot);
        }

        public TTarget Proxy()
        {
            return Proxy(ViaSet.Settings.DefaultWithDummyRoot);
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
        
        public IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return To().Retarget(target, optionsAction);
        }
        
        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return To().Record(optionsAction);
        }

        public IViaBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            return new ViaBuilder<TTarget>(RedirectRepository, RedirectBuilder<TTarget>.To(callConstraint));
        }

        public IFuncViaBuilder<TTarget, TReturn> To<TReturn>(Expression<IVia<TTarget>.StructReturnFunc<TTarget, TReturn>> constraintExpression) where TReturn : struct
        {
            var funcExpression = Expression.Lambda<Func<TTarget, TReturn>>(constraintExpression.Body, constraintExpression.Parameters);
            
            return new FuncViaBuilder<TTarget, TReturn>(RedirectRepository, RedirectBuilder<TTarget>.To(funcExpression));
        }

        public IClassFuncViaBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression) where TReturn : class
        {
            return new ClassFuncViaBuilder<TTarget, TReturn>(this, RedirectBuilder<TTarget>.To(constraintExpression));
        }

        public IActionViaBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            return new ActionViaBuilder<TTarget>(RedirectRepository, RedirectBuilder<TTarget>.To(constraintExpression));
        }
        
        public IActionViaBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>> constraintExpression)
        {
            return new ActionViaBuilder<TTarget>(RedirectRepository, RedirectBuilder<TTarget>.ToSet(memberExpression, constraintExpression));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IProxyCall<TTarget>? GetProxyCall()
        {
            var redirectPlan = RedirectRepository.RedirectPlan;

            return redirectPlan == RedirectPlan.Empty
                ? null
                : new ViaProxyCall<TTarget>(_relay, redirectPlan);
        }
    }
}