using System;
using System.Linq.Expressions;

namespace DivertR.Internal
{
    internal class DiverterRedirectBuilder<TTarget> : IDiverterRedirectBuilder<TTarget> where TTarget : class?
    {
        private readonly DiverterBuilder _diverterBuilder;

        public DiverterRedirectBuilder(IRedirect<TTarget> redirect, DiverterBuilder diverterBuilder)
        {
            Redirect = redirect;
            _diverterBuilder = diverterBuilder;
        }
        
        public IRedirect<TTarget> Redirect { get; }

        public IDiverterBuilder Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            Redirect.Via(via, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder ViaRedirect<TReturn>(Action<IViaOptionsBuilder>? optionsAction = null) where TReturn : class?
        {
            return ViaRedirect<TReturn>(null, optionsAction);
        }

        public IDiverterBuilder ViaRedirect<TReturn>(string? name, Action<IViaOptionsBuilder>? optionsAction = null) where TReturn : class?
        {
            var nestedRedirect = Redirect.RedirectSet.GetOrCreate<TReturn>(name);
            
            if (!_diverterBuilder.TryAddNestedRedirect(Redirect.RedirectId, nestedRedirect.RedirectId))
            {
                throw new DiverterException($"Nested redirect already registered {nestedRedirect.RedirectId}");
            }

            Redirect.ViaRedirect<TReturn>(name, DiverterBuilder.GetOptions(optionsAction));
            
            return _diverterBuilder;
        }

        public IDiverterBuilder Decorate<TReturn>(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            Redirect.Decorate(decorator, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Decorate<TReturn>(Func<TReturn, IDiverter, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var callConstraint = new ReturnCallConstraint(typeof(TReturn));
            var callHandler = _diverterBuilder.CreateDecoratorHandler(decorator);

            Redirect
                .To(callConstraint)
                .Via(callHandler, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            Redirect.Retarget(target, DiverterBuilder.GetOptions(optionsAction));
            
            return _diverterBuilder;
        }

        public IDiverterRedirectToBuilder<TTarget> To(ICallConstraint? callConstraint = null)
        {
            var redirectUpdater = Redirect.To(callConstraint);
            
            return new DiverterRedirectToBuilder<TTarget>(_diverterBuilder, redirectUpdater);
        }

        public IDiverterRedirectToBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, TReturn? _ = null) where TReturn : class?
        {
            var redirectUpdater = Redirect.To(constraintExpression);
            
            return new DiverterRedirectToClassBuilder<TTarget, TReturn>(_diverterBuilder, redirectUpdater);
        }

        public IDiverterRedirectToBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, TReturn? _ = null) where TReturn : struct
        {
            var redirectUpdater = Redirect.To(constraintExpression);
            
            return new DiverterRedirectToBuilder<TTarget, TReturn>(_diverterBuilder, redirectUpdater);
        }

        public IDiverterRedirectToVoidBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression)
        {
            var redirectUpdater = Redirect.To(constraintExpression);
            
            return new DiverterRedirectToVoidBuilder<TTarget>(_diverterBuilder, redirectUpdater);
        }

        public IDiverterRedirectToVoidBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null)
        {
            var redirectUpdater = Redirect.ToSet(memberExpression, constraintExpression);
            
            return new DiverterRedirectToVoidBuilder<TTarget>(_diverterBuilder, redirectUpdater);
        }
    }
}