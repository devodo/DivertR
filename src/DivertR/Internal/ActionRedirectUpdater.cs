using System;
using System.Collections;
using DivertR.Record;

namespace DivertR.Internal
{
    internal class ActionRedirectUpdater<TTarget> : RedirectUpdater<TTarget>, IActionRedirectUpdater<TTarget> where TTarget : class?
    {
        public ActionRedirectUpdater(IRedirect<TTarget> redirect, IActionViaBuilder<TTarget> redirectBuilder)
            : base(redirect, redirectBuilder)
        {
            ViaBuilder = redirectBuilder;
        }

        public new IActionViaBuilder<TTarget> ViaBuilder { get; }

        public new IActionRedirectUpdater<TTarget> Filter(ICallConstraint<TTarget> callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public IActionRedirectUpdater<TTarget> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IActionRedirectUpdater<TTarget> Via(Action<IActionRedirectCall<TTarget>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(viaDelegate);
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
            var builder = ViaBuilder.Args<TArgs>();
            
            return new ActionRedirectUpdater<TTarget, TArgs>(Redirect, builder);
        }

        public new IActionCallStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordVia = ViaBuilder.Record();
            InsertVia(recordVia.Via, optionsAction, disableSatisfyStrict: true);

            return recordVia.CallStream;
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
        public ActionRedirectUpdater(IRedirect<TTarget> redirect, IActionViaBuilder<TTarget, TArgs> redirectBuilder)
            : base(redirect, redirectBuilder)
        {
            ViaBuilder = redirectBuilder;
        }

        public new IActionViaBuilder<TTarget, TArgs> ViaBuilder { get; }

        public new IActionRedirectUpdater<TTarget, TArgs> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(viaDelegate, optionsAction);

            return this;
        }

        public IActionRedirectUpdater<TTarget, TArgs> Via(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(viaDelegate);
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
            var recordVia = ViaBuilder.Record();
            InsertVia(recordVia.Via, optionsAction, disableSatisfyStrict: true);

            return recordVia.CallStream;
        }
    }
}