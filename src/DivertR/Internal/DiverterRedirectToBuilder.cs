using System;

namespace DivertR.Internal
{
    internal class DiverterRedirectToBuilder<TTarget, TReturn> : IDiverterRedirectToBuilder<TTarget, TReturn> where TTarget : class?
    {
        protected readonly DiverterBuilder DiverterBuilder;
        protected readonly IFuncRedirectUpdater<TTarget, TReturn> RedirectUpdater;

        public DiverterRedirectToBuilder(DiverterBuilder diverterBuilder, IFuncRedirectUpdater<TTarget, TReturn> redirectUpdater)
        {
            DiverterBuilder = diverterBuilder;
            RedirectUpdater = redirectUpdater;
        }

        public IDiverterRedirectToBuilder<TTarget, TReturn> Filter(ICallConstraint callConstraint)
        {
            RedirectUpdater.Filter(callConstraint);

            return this;
        }

        public virtual IDiverterBuilder ViaRedirect(string? name = null)
        {
            throw new InvalidOperationException($"Invalid return type for Redirect. {typeof(TReturn)} is a struct type but only class types are valid.");
        }

        public IDiverterBuilder Decorate(Func<TReturn, TReturn> decorator)
        {
            RedirectUpdater
                .Decorate(decorator, DiverterBuilder.GetOptions());

            return DiverterBuilder;
        }

        public IDiverterBuilder Decorate(Func<TReturn, IDiverter, TReturn> decorator)
        {
            var callHandler = DiverterBuilder.CreateDecoratorHandler(decorator);
            RedirectUpdater.Via(callHandler, DiverterBuilder.GetOptions());

            return DiverterBuilder;
        }
    }
    
    internal class DiverterRedirectToClassBuilder<TTarget, TReturn> : DiverterRedirectToBuilder<TTarget, TReturn>
        where TTarget : class?
        where TReturn : class?
    {
        public DiverterRedirectToClassBuilder(DiverterBuilder diverterBuilder, IFuncRedirectUpdater<TTarget, TReturn> redirectUpdater)
            : base(diverterBuilder, redirectUpdater)
        {
        }

        public override IDiverterBuilder ViaRedirect(string? name = null)
        {
            RedirectUpdater.ViaRedirect(name, DiverterBuilder.GetOptions());

            return DiverterBuilder;
        }
    }
}