using System;
using System.Collections;
using DivertR.Record;

namespace DivertR.Internal
{
    internal class FuncViaBuilder<TTarget, TReturn> : ViaBuilder<TTarget>, IFuncViaBuilder<TTarget, TReturn> where TTarget : class
    {
        public FuncViaBuilder(IVia<TTarget> via, IFuncRedirectBuilder<TTarget, TReturn> redirectBuilder)
            : base(via, redirectBuilder)
        {
            RedirectBuilder = redirectBuilder;
        }

        public new IFuncRedirectBuilder<TTarget, TReturn> RedirectBuilder { get; }

        public new IFuncViaBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Redirect(() => instance, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate, optionsAction);
            Via.RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate, optionsAction);
            Via.RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate, optionsAction);
            Via.RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Redirect(instance, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IFuncViaBuilder<TTarget, TReturn> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Retarget<TArgs>(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Retarget(target, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var builder = RedirectBuilder.Args<TArgs>();
            
            return new FuncViaBuilder<TTarget, TReturn, TArgs>(Via, builder);
        }
        
        public new IFuncCallStream<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = RedirectBuilder.Record(optionsAction);
            Via.RedirectRepository.InsertRedirect(recordRedirect.Redirect);

            return recordRedirect.CallStream;
        }

        public IFuncCallStream<TTarget, TReturn, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Record(optionsAction);
        }
    }

    internal class FuncViaBuilder<TTarget, TReturn, TArgs> : FuncViaBuilder<TTarget, TReturn>, IFuncViaBuilder<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public FuncViaBuilder(IVia<TTarget> via, IFuncRedirectBuilder<TTarget, TReturn, TArgs> redirectBuilder)
            : base(via, redirectBuilder)
        {
            RedirectBuilder = redirectBuilder;
        }

        public new IFuncRedirectBuilder<TTarget, TReturn, TArgs> RedirectBuilder { get; }

        public new IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(instance, optionsAction);

            return this;
        }

        public new IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate, optionsAction);
            Via.RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate, optionsAction);
            Via.RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public new IFuncViaBuilder<TTarget, TReturn, TArgs> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public new IFuncCallStream<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = RedirectBuilder.Record(optionsAction);
            Via.RedirectRepository.InsertRedirect(recordRedirect.Redirect);

            return recordRedirect.CallStream;
        }
    }
}