using System;
using System.Collections;

namespace DivertR.Internal
{
    internal class DiverterRedirectToVoidBuilder<TTarget> : IDiverterRedirectToVoidBuilder<TTarget> where TTarget : class?
    {
        private readonly DiverterBuilder _diverterBuilder;
        private readonly IActionRedirectUpdater<TTarget> _redirectUpdater;

        public DiverterRedirectToVoidBuilder(DiverterBuilder diverterBuilder, IActionRedirectUpdater<TTarget> redirectUpdater)
        {
            _diverterBuilder = diverterBuilder;
            _redirectUpdater = redirectUpdater;
        }

        public IDiverterRedirectToVoidBuilder<TTarget> Filter(ICallConstraint callConstraint)
        {
            _redirectUpdater.Filter(callConstraint);

            return this;
        }

        public IDiverterBuilder Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectUpdater.Via(callHandler, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectUpdater.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Via(Action<IActionRedirectCall<TTarget>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectUpdater.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Via<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            _redirectUpdater.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectUpdater.Retarget(target, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }
    }
}