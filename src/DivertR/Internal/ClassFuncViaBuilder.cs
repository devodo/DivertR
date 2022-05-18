using System;
using System.Collections;

namespace DivertR.Internal
{
    internal class ClassFuncViaBuilder<TTarget, TReturn> : FuncViaBuilder<TTarget, TReturn>, IClassFuncViaBuilder<TTarget, TReturn>
        where TTarget : class
        where TReturn : class
    {
        private readonly IVia<TTarget> _via;

        public ClassFuncViaBuilder(IVia<TTarget> via, ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(via.RedirectRepository, callValidator, callConstraint)
        {
            _via = via;
        }

        public IVia<TReturn> Divert(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Divert(null!, optionsAction);
        }

        public IVia<TReturn> Divert(string name, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var via = _via.ViaSet.Via<TReturn>(name);
            ICallHandler<TTarget> callHandler = new DelegateCallHandler<TTarget>(call => via.Proxy((TReturn?) call.Relay.CallNext()));
            base.InsertRedirect(callHandler, optionsAction);

            return via;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(instance, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(instance, optionsAction);
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IClassFuncViaBuilder<TTarget, TReturn> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Retarget<TArgs>(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Retarget(target, optionsAction);
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ClassFuncViaBuilder<TTarget, TReturn, TArgs>(_via, CallValidator, CallConstraint);
        }
    }

    internal class ClassFuncViaBuilder<TTarget, TReturn, TArgs> : FuncViaBuilder<TTarget, TReturn, TArgs>,
        IClassFuncViaBuilder<TTarget, TReturn, TArgs>
        where TTarget : class
        where TReturn : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IVia<TTarget> _via;

        public ClassFuncViaBuilder(IVia<TTarget> via, ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(via.RedirectRepository, callValidator, callConstraint)
        {
            _via = via;
        }

        public IVia<TReturn> Divert(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Divert(null!, optionsAction);
        }

        public IVia<TReturn> Divert(string name, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var via = _via.ViaSet.Via<TReturn>(name);
            ICallHandler<TTarget> callHandler = new DelegateCallHandler<TTarget>(call => via.Proxy((TReturn?) call.Relay.CallNext()));
            base.InsertRedirect(callHandler, optionsAction);

            return via;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(instance, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }
    }
}