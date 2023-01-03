using System;
using System.Collections;
using DivertR.Record;

namespace DivertR.Internal
{
    internal class FuncRedirectUpdater<TTarget, TReturn> : RedirectUpdater<TTarget>, IFuncRedirectUpdater<TTarget, TReturn> where TTarget : class?
    {
        public FuncRedirectUpdater(IRedirect<TTarget> redirect, IFuncViaBuilder<TTarget, TReturn> redirectBuilder)
            : base(redirect, redirectBuilder)
        {
            ViaBuilder = redirectBuilder;
        }

        public new IFuncViaBuilder<TTarget, TReturn> ViaBuilder { get; }

        public new IFuncRedirectUpdater<TTarget, TReturn> Filter(ICallConstraint<TTarget> callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public IFuncRedirectUpdater<TTarget, TReturn> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            return Via(() => instance, optionsAction);
        }

        public IFuncRedirectUpdater<TTarget, TReturn> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IFuncRedirectUpdater<TTarget, TReturn> Via(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via<TArgs>(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Via(instance, optionsAction);
        }

        public IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via<TArgs>(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Via(viaDelegate, optionsAction);
        }

        public IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Via(viaDelegate, optionsAction);
        }

        public new IFuncRedirectUpdater<TTarget, TReturn> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public IFuncRedirectUpdater<TTarget, TReturn, TArgs> Retarget<TArgs>(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Retarget(target, optionsAction);
        }

        public IFuncRedirectUpdater<TTarget, TReturn, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var builder = ViaBuilder.Args<TArgs>();
            
            return new FuncRedirectUpdater<TTarget, TReturn, TArgs>(Redirect, builder);
        }
        
        public new IFuncCallStream<TTarget, TReturn> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordVia = ViaBuilder.Record();
            InsertVia(recordVia.Via, optionsAction, disableSatisfyStrict: true);

            return recordVia.CallStream;
        }

        public IFuncCallStream<TTarget, TReturn, TArgs> Record<TArgs>(Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Record(optionsAction);
        }
    }

    internal class FuncRedirectUpdater<TTarget, TReturn, TArgs> : FuncRedirectUpdater<TTarget, TReturn>, IFuncRedirectUpdater<TTarget, TReturn, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public FuncRedirectUpdater(IRedirect<TTarget> redirect, IFuncViaBuilder<TTarget, TReturn, TArgs> redirectBuilder)
            : base(redirect, redirectBuilder)
        {
            ViaBuilder = redirectBuilder;
        }

        public new IFuncViaBuilder<TTarget, TReturn, TArgs> ViaBuilder { get; }

        public new IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(instance, optionsAction);

            return this;
        }

        public new IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(viaDelegate, optionsAction);

            return this;
        }

        public IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public new IFuncRedirectUpdater<TTarget, TReturn, TArgs> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public new IFuncCallStream<TTarget, TReturn, TArgs> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordVia = ViaBuilder.Record();
            InsertVia(recordVia.Via, optionsAction, disableSatisfyStrict: true);

            return recordVia.CallStream;
        }
    }
}