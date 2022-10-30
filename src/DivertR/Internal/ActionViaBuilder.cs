﻿using System;
using System.Collections;
using DivertR.Record;

namespace DivertR.Internal
{
    internal class ActionViaBuilder<TTarget> : ViaBuilder<TTarget>, IActionViaBuilder<TTarget> where TTarget : class?
    {
        public ActionViaBuilder(IVia<TTarget> via, IActionRedirectBuilder<TTarget> redirectBuilder)
            : base(via, redirectBuilder)
        {
            RedirectBuilder = redirectBuilder;
        }

        public new IActionRedirectBuilder<TTarget> RedirectBuilder { get; }

        public new IActionViaBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public IActionViaBuilder<TTarget> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate);
            InsertRedirect(redirect, optionsAction);

            return this;
        }

        public IActionViaBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate);
            InsertRedirect(redirect, optionsAction);
            
            return this;
        }

        public IActionViaBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate);
            InsertRedirect(redirect, optionsAction);
            
            return this;
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IActionViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public IActionViaBuilder<TTarget, TArgs> Retarget<TArgs>(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Retarget(target, optionsAction);
        }

        public IActionViaBuilder<TTarget, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var builder = RedirectBuilder.Args<TArgs>();
            
            return new ActionViaBuilder<TTarget, TArgs>(Via, builder);
        }

        public new IActionCallStream<TTarget> Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var recordRedirect = RedirectBuilder.Record();
            InsertRedirect(recordRedirect.Redirect, optionsAction, disableSatisfyStrict: true);

            return recordRedirect.CallStream;
        }

        public IActionCallStream<TTarget, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Record(optionsAction);
        }
    }

    internal class ActionViaBuilder<TTarget, TArgs> : ActionViaBuilder<TTarget>, IActionViaBuilder<TTarget, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public ActionViaBuilder(IVia<TTarget> via, IActionRedirectBuilder<TTarget, TArgs> redirectBuilder)
            : base(via, redirectBuilder)
        {
            RedirectBuilder = redirectBuilder;
        }

        public new IActionRedirectBuilder<TTarget, TArgs> RedirectBuilder { get; }

        public new IActionViaBuilder<TTarget, TArgs> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate);
            InsertRedirect(redirect, optionsAction);
            
            return this;
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate);
            InsertRedirect(redirect, optionsAction);
            
            return this;
        }

        public new IActionViaBuilder<TTarget, TArgs> Retarget(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public new IActionCallStream<TTarget, TArgs> Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var recordRedirect = RedirectBuilder.Record();
            InsertRedirect(recordRedirect.Redirect, optionsAction, disableSatisfyStrict: true);

            return recordRedirect.CallStream;
        }
    }
}