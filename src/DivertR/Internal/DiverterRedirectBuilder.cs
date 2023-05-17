using System;
using System.Linq.Expressions;

namespace DivertR.Internal
{
    internal class DiverterRedirectBuilder<TTarget> : IDiverterRedirectBuilder<TTarget> where TTarget : class?
    {
        private readonly IRedirect<TTarget> _redirect;
        private readonly DiverterBuilder _diverterBuilder;

        public DiverterRedirectBuilder(IRedirect<TTarget> redirect, DiverterBuilder diverterBuilder)
        {
            _redirect = redirect;
            _diverterBuilder = diverterBuilder;
        }

        public IDiverterBuilder ViaRedirect<TReturn>(string? name = null) where TReturn : class?
        {
            var nestedRedirect = _redirect.RedirectSet.GetOrCreate<TReturn>(name);
            
            if (!_diverterBuilder.TryAddNestedRedirect(_redirect.RedirectId, nestedRedirect.RedirectId))
            {
                throw new DiverterException($"Nested redirect already registered {nestedRedirect.RedirectId}");
            }

            _redirect.ViaRedirect<TReturn>(name, DiverterBuilder.GetOptions());
            
            return _diverterBuilder;
        }

        public IDiverterBuilder Decorate<TReturn>(Func<TReturn, TReturn> decorator)
        {
            _redirect.Decorate(decorator, DiverterBuilder.GetOptions());

            return _diverterBuilder;
        }

        public IDiverterBuilder Decorate<TReturn>(Func<TReturn, IDiverter, TReturn> decorator)
        {
            var callConstraint = new ReturnCallConstraint(typeof(TReturn));
            var callHandler = _diverterBuilder.CreateDecoratorHandler(decorator);

            _redirect
                .To(callConstraint)
                .Via(callHandler, DiverterBuilder.GetOptions());

            return _diverterBuilder;
        }

        public IDiverterRedirectToBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, TReturn? _ = null) where TReturn : class?
        {
            var redirectUpdater = _redirect.To(constraintExpression);
            
            return new DiverterRedirectToClassBuilder<TTarget, TReturn>(_diverterBuilder, redirectUpdater);
        }

        public IDiverterRedirectToBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, TReturn? _ = null) where TReturn : struct
        {
            var redirectUpdater = _redirect.To(constraintExpression);
            
            return new DiverterRedirectToBuilder<TTarget, TReturn>(_diverterBuilder, redirectUpdater);
        }
    }
}