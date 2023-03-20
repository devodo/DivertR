using System;
using System.Collections;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class ActionRedirectUpdater<TTarget> : RedirectUpdater<TTarget>, IActionRedirectUpdater<TTarget> where TTarget : class?
    {
        private readonly ActionViaBuilder<TTarget> _viaBuilder;
        
        public ActionRedirectUpdater(IRedirect<TTarget> redirect, ActionViaBuilder<TTarget> redirectBuilder)
            : base(redirect, redirectBuilder)
        {
            _viaBuilder = redirectBuilder;
        }
        
        public new IActionRedirectUpdater<TTarget> Filter(ICallConstraint callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public IActionRedirectUpdater<TTarget> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IActionRedirectUpdater<TTarget> Via(Action<IActionRedirectCall<TTarget>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);
            
            return this;
        }

        public IActionRedirectUpdater<TTarget, TArgs> Via<TArgs>(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Via(viaDelegate, optionsAction);
        }

        public IActionRedirectUpdater<TTarget, TArgs> Via<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Via(viaDelegate, optionsAction);
        }

        public new IActionRedirectUpdater<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public IActionRedirectUpdater<TTarget, TArgs> Retarget<TArgs>(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Retarget(target, optionsAction);
        }

        public IActionRedirectUpdater<TTarget, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var builder = _viaBuilder.Args<TArgs>();
            
            return new ActionRedirectUpdater<TTarget, TArgs>(Redirect, builder);
        }

        public new IActionCallStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordStream = base.Record(optionsAction);
            var callStream = new ActionCallStream<TTarget>(recordStream, _viaBuilder.CallValidator);
            
            return callStream;
        }

        public IActionCallStream<TTarget, TArgs> Record<TArgs>(Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Record(optionsAction);
        }
    }

    internal class ActionRedirectUpdater<TTarget, TArgs> : ActionRedirectUpdater<TTarget>, IActionRedirectUpdater<TTarget, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly ActionViaBuilder<TTarget, TArgs> _viaBuilder;
        
        public ActionRedirectUpdater(IRedirect<TTarget> redirect, ActionViaBuilder<TTarget, TArgs> redirectBuilder)
            : base(redirect, redirectBuilder)
        {
            _viaBuilder = redirectBuilder;
        }

        public new IActionRedirectUpdater<TTarget, TArgs> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(viaDelegate, optionsAction);

            return this;
        }

        public IActionRedirectUpdater<TTarget, TArgs> Via(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);
            
            return this;
        }

        public new IActionRedirectUpdater<TTarget, TArgs> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public new IActionCallStream<TTarget, TArgs> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            return base.Record().Args<TArgs>();
        }
    }
}