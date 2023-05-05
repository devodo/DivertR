using System;
using System.Linq.Expressions;

namespace DivertR.Internal
{
    internal class NestedRegisterBuilder<TTarget> : INestedRegisterBuilder<TTarget> where TTarget : class?
    {
        private readonly IRedirect<TTarget> _redirect;
        private readonly DiverterBuilder _diverterBuilder;

        public NestedRegisterBuilder(IRedirect<TTarget> redirect, DiverterBuilder diverterBuilder)
        {
            _redirect = redirect;
            _diverterBuilder = diverterBuilder;
        }

        public INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?
        {
            return ThenRedirect((string?) null, registerAction);
        }

        public INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(string? name, Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?
        {
            var nestedRedirect = _redirect.RedirectSet.GetOrCreate<TReturn>(name);
            
            if (!_diverterBuilder.TryAddNestedRedirect(_redirect.RedirectId, nestedRedirect.RedirectId))
            {
                throw new DiverterException($"Nested redirect already registered {nestedRedirect.RedirectId}");
            }

            _redirect.ViaRedirect<TReturn>(name, opt =>
            {
                opt.DisableSatisfyStrict();
                opt.Persist();
            });

            registerAction?.Invoke(new NestedRegisterBuilder<TReturn>(nestedRedirect, _diverterBuilder));

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
            
            registerAction?.Invoke(new NestedRegisterBuilder<TReturn>(nestedRedirect, _diverterBuilder));

            return this;
        }
    }
}