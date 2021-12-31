using System;
using System.Collections;

namespace DivertR.Internal
{
    internal class ClassFuncRedirectBuilder<TTarget, TReturn> : FuncRedirectBuilder<TTarget, TReturn>, IClassFuncRedirectBuilder<TTarget, TReturn>
        where TTarget : class
        where TReturn : class
    {
        public ClassFuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression) : base(via, parsedCallExpression)
        {
        }

        public IVia<TReturn> Divert(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Divert(null!, optionsAction);
        }

        public IVia<TReturn> Divert(string name, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var via = Via.ViaSet.Via<TReturn>(name);
            ICallHandler<TTarget> callHandler = new DelegateCallHandler<TTarget>(callInfo => via.Proxy((TReturn?) Via.Relay.CallNext()));
            base.InsertRedirect(callHandler, optionsAction);

            return via;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(instance, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(instance, optionsAction);
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ClassFuncRedirectBuilder<TTarget, TReturn, TArgs>(Via, ParsedCallExpression, CallConstraint, Relay);
        }
    }

    internal class ClassFuncRedirectBuilder<TTarget, TReturn, TArgs> : FuncRedirectBuilder<TTarget, TReturn, TArgs>,
        IClassFuncRedirectBuilder<TTarget, TReturn, TArgs>
        where TTarget : class
        where TReturn : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public ClassFuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint, Relay<TTarget, TReturn> relay)
            : base(via, parsedCallExpression, callConstraint, relay)
        {
        }

        public IVia<TReturn> Divert(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Divert(null!, optionsAction);
        }

        public IVia<TReturn> Divert(string name, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var via = Via.ViaSet.Via<TReturn>(name);
            ICallHandler<TTarget> callHandler = new DelegateCallHandler<TTarget>(callInfo => via.Proxy((TReturn?) Via.Relay.CallNext()));
            base.InsertRedirect(callHandler, optionsAction);

            return via;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(instance, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IClassFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }
    }
}