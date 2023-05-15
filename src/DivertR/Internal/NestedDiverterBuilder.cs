using System;
using System.Linq.Expressions;

namespace DivertR.Internal
{
    internal class NestedDiverterBuilder<TTarget> : INestedDiverterBuilder<TTarget> where TTarget : class?
    {
        private readonly IRedirect<TTarget> _redirect;
        private readonly DiverterBuilder _diverterBuilder;

        public NestedDiverterBuilder(IRedirect<TTarget> redirect, DiverterBuilder diverterBuilder)
        {
            _redirect = redirect;
            _diverterBuilder = diverterBuilder;
        }

        public INestedDiverterBuilder<TTarget> AddRedirect<TReturn>(Action<INestedDiverterBuilder<TReturn>>? registerAction = null) where TReturn : class?
        {
            return AddRedirect((string?) null, registerAction);
        }

        public INestedDiverterBuilder<TTarget> AddRedirect<TReturn>(string? name, Action<INestedDiverterBuilder<TReturn>>? registerAction = null) where TReturn : class?
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

            registerAction?.Invoke(new NestedDiverterBuilder<TReturn>(nestedRedirect, _diverterBuilder));

            return this;
        }

        public INestedDiverterBuilder<TTarget> AddRedirect<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, Action<INestedDiverterBuilder<TReturn>>? registerAction = null) where TReturn : class?
        {
            return AddRedirect(null, constraintExpression, registerAction);
        }

        public INestedDiverterBuilder<TTarget> AddRedirect<TReturn>(string? name, Expression<Func<TTarget, TReturn>> constraintExpression, Action<INestedDiverterBuilder<TReturn>>? registerAction = null) where TReturn : class?
        {
            var nestedRedirect = _redirect
                .To(constraintExpression)
                .ViaRedirect(name, opt =>
                {
                    opt.DisableSatisfyStrict();
                    opt.Persist();
                });
            
            registerAction?.Invoke(new NestedDiverterBuilder<TReturn>(nestedRedirect, _diverterBuilder));

            return this;
        }
    }
}