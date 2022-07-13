using System;
using System.Collections;
using DivertR.Record;

namespace DivertR.Internal
{
    internal class FuncViaBuilder<TTarget, TReturn> : DelegateViaBuilder<TTarget>, IFuncViaBuilder<TTarget, TReturn> where TTarget : class
    {
        private readonly IFuncRedirectBuilder<TTarget, TReturn> _redirectBuilder;

        public FuncViaBuilder(IRedirectRepository redirectRepository, IFuncRedirectBuilder<TTarget, TReturn> redirectBuilder)
            : base(redirectRepository, redirectBuilder)
        {
            _redirectBuilder = redirectBuilder;
        }
        
        public new IFuncViaBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }

        public new IFuncViaBuilder<TTarget, TReturn> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Redirect(() => instance, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(instance, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IFuncViaBuilder<TTarget, TReturn> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Retarget<TArgs>(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Retarget(target, optionsAction);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var builder = _redirectBuilder.WithArgs<TArgs>();
            
            return new FuncViaBuilder<TTarget, TReturn, TArgs>(RedirectRepository, builder);
        }
        
        public new IFuncCallStream<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = _redirectBuilder.Record(optionsAction);
            RedirectRepository.InsertRedirect(recordRedirect.Redirect);

            return recordRedirect.CallStream;
        }

        public IFuncCallStream<TTarget, TReturn, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Record(optionsAction);
        }
    }

    internal class FuncViaBuilder<TTarget, TReturn, TArgs> : FuncViaBuilder<TTarget, TReturn>, IFuncViaBuilder<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IFuncRedirectBuilder<TTarget, TReturn, TArgs> _redirectBuilder;

        public FuncViaBuilder(IRedirectRepository redirectRepository, IFuncRedirectBuilder<TTarget, TReturn, TArgs> redirectBuilder)
            : base(redirectRepository, redirectBuilder)
        {
            _redirectBuilder = redirectBuilder;
        }

        public new IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

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
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public new IFuncViaBuilder<TTarget, TReturn, TArgs> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public new IFuncCallStream<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = _redirectBuilder.Record(optionsAction);
            RedirectRepository.InsertRedirect(recordRedirect.Redirect);

            return recordRedirect.CallStream;
        }
    }
}