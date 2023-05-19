using System;
using System.Collections;

namespace DivertR.Internal
{
    internal class DiverterRedirectToActionBuilder<TTarget> : IDiverterRedirectToActionBuilder<TTarget> where TTarget : class?
    {
        private readonly DiverterBuilder _diverterBuilder;
        private readonly IRedirectToActionBuilder<TTarget> _redirectBuilder;

        public DiverterRedirectToActionBuilder(DiverterBuilder diverterBuilder, IRedirectToActionBuilder<TTarget> redirectBuilder)
        {
            _diverterBuilder = diverterBuilder;
            _redirectBuilder = redirectBuilder;
        }

        public IDiverterRedirectToActionBuilder<TTarget> Filter(ICallConstraint callConstraint)
        {
            _redirectBuilder.Filter(callConstraint);

            return this;
        }

        public IDiverterBuilder Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectBuilder.Via(callHandler, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectBuilder.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Via(Action<IActionRedirectCall<TTarget>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectBuilder.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Via<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            _redirectBuilder.Via(viaDelegate, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }

        public IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            _redirectBuilder.Retarget(target, DiverterBuilder.GetOptions(optionsAction));

            return _diverterBuilder;
        }
    }
}