using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;
using DivertR.DispatchProxy;
using DivertR.Internal;

namespace DivertR
{
    public class Via<TTarget> : IVia<TTarget> where TTarget : class
    {
        private readonly ViaStateRepository _viaStateRepository;
        private readonly IProxyFactory _proxyFactory;
        private readonly RelayState<TTarget> _relayState;
        private readonly Lazy<IRelay<TTarget>> _relay;

        public Via() : this(ViaId.From<TTarget>(), new ViaStateRepository())
        {
        }
        
        internal Via(ViaId viaId, ViaStateRepository viaStateRepository)
        {
            if (!typeof(TTarget).IsInterface && !typeof(TTarget).IsClass)
            {
                throw new ArgumentException("Only interface and types are supported", typeof(TTarget).Name);
            }
            
            ViaId = viaId;
            _viaStateRepository = viaStateRepository;
            _proxyFactory = DispatchProxyFactory.Instance;
            _relayState = new RelayState<TTarget>();
            _relay = new Lazy<IRelay<TTarget>>(() => new Relay<TTarget>(_relayState, _proxyFactory));
        }

        public ViaId ViaId { get; }

        public IRelay<TTarget> Relay => _relay.Value;
        public TTarget Next => Relay.Next;

        public TTarget Proxy(TTarget? original = null)
        {
            ViaState<TTarget>? GetRedirectRoute()
            {
                return _viaStateRepository.Get<TTarget>(ViaId);
            }

            return _proxyFactory.CreateDiverterProxy(original, GetRedirectRoute);
        }

        public object ProxyObject(object? original = null)
        {
            if (original != null && !(original is TTarget))
            {
                throw new ArgumentException($"Not assignable to {typeof(TTarget).Name}", nameof(original));
            }

            return Proxy(original as TTarget);
        }
        
        public IVia<TTarget> RedirectTo(TTarget target, object? state = null)
        {
            return Redirect().To(target, state);
        }

        public IRedirectBuilder<TTarget> Redirect(ICallConstraint? callCondition = null)
        {
            return new RedirectBuilder<TTarget>(this, callCondition);
        }
        
        public IFuncRedirectBuilder<TTarget, TReturn> Redirect<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null)
            {
                throw new ArgumentNullException(nameof(lambdaExpression));
            }
            
            return lambdaExpression.Body switch
            {
                MethodCallExpression methodExpression => new FuncRedirectBuilder<TTarget, TReturn>(this, methodExpression),
                MemberExpression propertyExpression => new FuncRedirectBuilder<TTarget, TReturn>(this, propertyExpression),
                _ => throw new ArgumentException($"Invalid expression type: {lambdaExpression.Body.GetType()}", nameof(lambdaExpression))
            };
        }
        
        public IActionRedirectBuilder<TTarget> Redirect(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null)
            {
                throw new ArgumentNullException(nameof(lambdaExpression));
            }
            
            return lambdaExpression.Body switch
            {
                MethodCallExpression methodExpression => new ActionRedirectBuilder<TTarget>(this, methodExpression),
                MemberExpression propertyExpression => new ActionRedirectBuilder<TTarget>(this, propertyExpression),
                _ => throw new ArgumentException($"Invalid expression type: {lambdaExpression.Body.GetType()}", nameof(lambdaExpression))
            };
        }
        
        public IActionRedirectBuilder<TTarget> RedirectSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));
            if (valueExpression?.Body == null) throw new ArgumentNullException(nameof(valueExpression));

            if (!(lambdaExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Only property member expressions are valid input to RedirectSet", nameof(propertyExpression));
            }

            return new ActionRedirectBuilder<TTarget>(this, propertyExpression, valueExpression.Body);
        }

        public IVia<TTarget> AddRedirect(IRedirect<TTarget> redirect)
        {
            ViaState<TTarget> Create()
            {
                return new ViaState<TTarget>(redirect, _relayState);
            }

            ViaState<TTarget> Update(ViaState<TTarget> existing)
            {
                var redirects = existing.Redirects.ToList();
                redirects.Add(redirect);
                return new ViaState<TTarget>(redirects, _relayState);
            }

            _viaStateRepository.AddOrUpdate(ViaId, Create, Update);

            return this;
        }

        public IVia<TTarget> InsertRedirect(int index, TTarget target, object? state = null)
        {
            var redirect = new TargetRedirect<TTarget>(target, state);
            
            ViaState<TTarget> Create()
            {
                if (index != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0 if there are no existing redirects");
                }
                
                return new ViaState<TTarget>(redirect, _relayState);
            }

            ViaState<TTarget> Update(ViaState<TTarget> existing)
            {
                if (index < 0 || index > existing.Redirects.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the existing redirects");
                }
                
                var redirects = existing.Redirects.ToList();
                redirects.Insert(index, redirect);
                return new ViaState<TTarget>(redirects, _relayState);
            }

            _viaStateRepository.AddOrUpdate(ViaId, Create, Update);

            return this;
        }

        public IVia<TTarget> Reset()
        {
            _viaStateRepository.Reset(ViaId);

            return this;
        }
    }
}