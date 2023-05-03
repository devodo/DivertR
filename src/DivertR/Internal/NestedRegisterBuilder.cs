using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace DivertR.Internal
{
    internal class NestedRegisterBuilder<TTarget> : INestedRegisterBuilder<TTarget> where TTarget : class?
    {
        private readonly IRedirect<TTarget> _redirect;
        private readonly ConcurrentDictionary<RedirectId, ConcurrentDictionary<RedirectId, IRedirect>> _registeredRedirects;

        public NestedRegisterBuilder(IRedirect<TTarget> redirect, ConcurrentDictionary<RedirectId, ConcurrentDictionary<RedirectId, IRedirect>> registeredRedirects)
        {
            _redirect = redirect;
            _registeredRedirects = registeredRedirects;
        }

        public INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?
        {
            return ThenRedirect((string?) null, registerAction);
        }

        public INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(string? name, Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?
        {
            var nestedRedirect = _redirect.RedirectSet.GetOrCreate<TReturn>(name);
            
            if (!TryAddNestedRedirect(nestedRedirect))
            {
                throw new DiverterException($"Nested redirect already registered {nestedRedirect.RedirectId}");
            }

            _redirect.ViaRedirect<TReturn>(name, opt =>
            {
                opt.DisableSatisfyStrict();
                opt.Persist();
            });

            registerAction?.Invoke(new NestedRegisterBuilder<TReturn>(nestedRedirect, _registeredRedirects));

            return this;
        }

        public INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?
        {
            return ThenRedirect(null, constraintExpression, registerAction);
        }

        public INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(string? name, Expression<Func<TTarget, TReturn>> constraintExpression, Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?
        {
            var nestedRedirect = _redirect
                .To(constraintExpression)
                .ViaRedirect(name, opt =>
                {
                    opt.DisableSatisfyStrict();
                    opt.Persist();
                });
            
            registerAction?.Invoke(new NestedRegisterBuilder<TReturn>(nestedRedirect, _registeredRedirects));

            return this;
        }

        public INestedRegisterBuilder<TTarget> ThenDecorate<TReturn>(Func<TReturn, TReturn> decorator)
        {
            _redirect.ViaDecorator(decorator, opt =>
            {
                opt.DisableSatisfyStrict();
                opt.Persist();
            });

            return this;
        }

        private bool TryAddNestedRedirect(IRedirect nestedRedirect)
        {
            var registered = _registeredRedirects.GetOrAdd(_redirect.RedirectId,
                _ => new ConcurrentDictionary<RedirectId, IRedirect>());

            return registered.TryAdd(nestedRedirect.RedirectId, nestedRedirect);
        }
    }
}