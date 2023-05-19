using System;
using System.Collections;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectToActionBuilder<TTarget> : RedirectToBuilder<TTarget>, IRedirectToActionBuilder<TTarget> where TTarget : class?
    {
        private readonly ActionViaBuilder<TTarget> _viaBuilder;
        
        public RedirectToActionBuilder(Redirect<TTarget> redirect, ActionViaBuilder<TTarget> viaBuilder)
            : base(redirect, viaBuilder)
        {
            _viaBuilder = viaBuilder;
        }
        
        public new IRedirectToActionBuilder<TTarget> Filter(ICallConstraint callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public new IRedirectToActionBuilder<TTarget> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(callHandler, optionsAction);

            return this;
        }

        public IRedirectToActionBuilder<TTarget> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectToActionBuilder<TTarget> Via(Action<IActionRedirectCall<TTarget>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);
            
            return this;
        }

        public IRedirectToActionBuilder<TTarget, TArgs> Via<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Via(viaDelegate, optionsAction);
        }

        public new IRedirectToActionBuilder<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public IRedirectToActionBuilder<TTarget, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var builder = _viaBuilder.Args<TArgs>();
            
            return new RedirectToActionBuilder<TTarget, TArgs>(Redirect, builder);
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

    internal class RedirectToActionBuilder<TTarget, TArgs> : RedirectToActionBuilder<TTarget>, IRedirectToActionBuilder<TTarget, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly ActionViaBuilder<TTarget, TArgs> _viaBuilder;
        
        public RedirectToActionBuilder(Redirect<TTarget> redirect, ActionViaBuilder<TTarget, TArgs> viaBuilder)
            : base(redirect, viaBuilder)
        {
            _viaBuilder = viaBuilder;
        }

        public new IRedirectToActionBuilder<TTarget, TArgs> Filter(ICallConstraint callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public new IRedirectToActionBuilder<TTarget, TArgs> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(callHandler, optionsAction);

            return this;
        }

        public new IRedirectToActionBuilder<TTarget, TArgs> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(viaDelegate, optionsAction);

            return this;
        }

        public IRedirectToActionBuilder<TTarget, TArgs> Via(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);
            
            return this;
        }

        public new IRedirectToActionBuilder<TTarget, TArgs> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
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