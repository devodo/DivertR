using System;
using System.Collections;

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

        public IDiverterBuilder Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectUpdater.Via(callHandler, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }

        public IDiverterBuilder Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectUpdater.Via(instance, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }

        public IDiverterBuilder Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectUpdater.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }

        public IDiverterBuilder Via(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectUpdater.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }
        
        public IDiverterBuilder Via<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            RedirectUpdater.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }

        public IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectUpdater.Retarget(target, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }

        public IDiverterBuilder ViaRedirect(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            return ViaRedirect(null, optionsAction);
        }

        public virtual IDiverterBuilder ViaRedirect(string? name, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            throw new InvalidOperationException($"Invalid return type for Redirect. {typeof(TReturn)} is a struct type but only class types are valid.");
        }

        public IDiverterBuilder Decorate(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectUpdater.Decorate(decorator, DiverterBuilder.GetOptions(optionsAction));

            return DiverterBuilder;
        }

        public IDiverterBuilder Decorate(Func<TReturn, IDiverter, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var callHandler = DiverterBuilder.CreateDecoratorHandler(decorator);
            RedirectUpdater.Via(callHandler, DiverterBuilder.GetOptions(optionsAction));

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

        public override IDiverterBuilder ViaRedirect(string? name, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectUpdater.ViaRedirect(name, DiverterBuilder.GetOptions(optionsAction));

            return DiverterBuilder;
        }
    }

    internal class DiverterRedirectToBuilder<TTarget> : IDiverterRedirectToBuilder<TTarget> where TTarget : class?
    {
        private readonly DiverterBuilder _diverterBuilder;
        private readonly IRedirectUpdater<TTarget> _redirectUpdater;

        public DiverterRedirectToBuilder(DiverterBuilder diverterBuilder, IRedirectUpdater<TTarget> redirectUpdater)
        {
            _diverterBuilder = diverterBuilder;
            _redirectUpdater = redirectUpdater;
        }
        
        public IDiverterRedirectToBuilder<TTarget> Filter(ICallConstraint callConstraint)
        {
            _redirectUpdater.Filter(callConstraint);

            return this;
        }

        public IDiverterBuilder Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectUpdater.Via(callHandler, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectUpdater.Retarget(target, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }
    }
}