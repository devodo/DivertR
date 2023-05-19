using System;
using System.Collections;

namespace DivertR.Internal
{
    internal class DiverterRedirectToFuncBuilder<TTarget, TReturn> : IDiverterRedirectToFuncBuilder<TTarget, TReturn> where TTarget : class?
    {
        protected readonly DiverterBuilder DiverterBuilder;
        protected readonly IRedirectToFuncBuilder<TTarget, TReturn> RedirectBuilder;

        public DiverterRedirectToFuncBuilder(DiverterBuilder diverterBuilder, IRedirectToFuncBuilder<TTarget, TReturn> redirectBuilder)
        {
            DiverterBuilder = diverterBuilder;
            RedirectBuilder = redirectBuilder;
        }

        public IDiverterRedirectToFuncBuilder<TTarget, TReturn> Filter(ICallConstraint callConstraint)
        {
            RedirectBuilder.Filter(callConstraint);

            return this;
        }

        public IDiverterBuilder Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectBuilder.Via(callHandler, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }

        public IDiverterBuilder Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectBuilder.Via(instance, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }

        public IDiverterBuilder Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectBuilder.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }

        public IDiverterBuilder Via(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectBuilder.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }
        
        public IDiverterBuilder Via<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            RedirectBuilder.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));
            
            return DiverterBuilder;
        }

        public IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectBuilder.Retarget(target, DiverterBuilder.GetOptions(optionsAction));
            
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
            RedirectBuilder.Decorate(decorator, DiverterBuilder.GetOptions(optionsAction));

            return DiverterBuilder;
        }

        public IDiverterBuilder Decorate(Func<TReturn, IDiverter, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var callHandler = DiverterBuilder.CreateDecoratorHandler(decorator);
            RedirectBuilder.Via(callHandler, DiverterBuilder.GetOptions(optionsAction));

            return DiverterBuilder;
        }
    }
    
    internal class DiverterRedirectToFuncClassBuilder<TTarget, TReturn> : DiverterRedirectToFuncBuilder<TTarget, TReturn>
        where TTarget : class?
        where TReturn : class?
    {
        public DiverterRedirectToFuncClassBuilder(DiverterBuilder diverterBuilder, IRedirectToFuncBuilder<TTarget, TReturn> redirectBuilder)
            : base(diverterBuilder, redirectBuilder)
        {
        }

        public override IDiverterBuilder ViaRedirect(string? name, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            RedirectBuilder.ViaRedirect(name, DiverterBuilder.GetOptions(optionsAction));

            return DiverterBuilder;
        }
    }
}