using System;

namespace DivertR.Internal
{
    internal class DiverterRedirectToBuilder<TTarget> : IDiverterRedirectToBuilder<TTarget> where TTarget : class?
    {
        private readonly DiverterBuilder _diverterBuilder;
        private readonly IRedirectToBuilder<TTarget> _redirectToBuilder;

        public DiverterRedirectToBuilder(DiverterBuilder diverterBuilder, IRedirectToBuilder<TTarget> redirectToBuilder)
        {
            _diverterBuilder = diverterBuilder;
            _redirectToBuilder = redirectToBuilder;
        }
        
        public IDiverterRedirectToBuilder<TTarget> Filter(ICallConstraint callConstraint)
        {
            _redirectToBuilder.Filter(callConstraint);

            return this;
        }

        public IDiverterBuilder Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectToBuilder.Via(callHandler, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectToBuilder.Retarget(target, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }
    }
}