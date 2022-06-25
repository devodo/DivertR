using System;
using System.Collections;
using System.Linq;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class ActionViaBuilder<TTarget> : DelegateViaBuilder<TTarget>, IActionViaBuilder<TTarget> where TTarget : class
    {
        private readonly IActionRedirectBuilder<TTarget> _redirectBuilder;
        private readonly ICallValidator _callValidator;

        public ActionViaBuilder(IRedirectRepository redirectRepository, IActionRedirectBuilder<TTarget> redirectBuilder, ICallValidator callValidator)
            : base(redirectRepository, redirectBuilder)
        {
            _redirectBuilder = redirectBuilder;
            _callValidator = callValidator;
        }
        
        public new IActionViaBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }
        
        public new IActionViaBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public IActionViaBuilder<TTarget> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IActionViaBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);
            
            return this;
        }

        public IActionViaBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);
            
            return this;
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public new IActionViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public IActionViaBuilder<TTarget, TArgs> Retarget<TArgs>(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Retarget(target, optionsAction);
        }

        public IActionViaBuilder<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var builder = _redirectBuilder.WithArgs<TArgs>();
            
            return new ActionViaBuilder<TTarget, TArgs>(RedirectRepository, builder, _callValidator);
        }

        public new IActionCallStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((ViaBuilder<TTarget>) this).Record(optionsAction);

            return new ActionCallStream<TTarget>(recordStream, _callValidator);
        }

        public IActionCallStream<TTarget, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Record(optionsAction);
        }
    }

    internal class ActionViaBuilder<TTarget, TArgs> : ActionViaBuilder<TTarget>, IActionViaBuilder<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IActionRedirectBuilder<TTarget, TArgs> _redirectBuilder;

        public ActionViaBuilder(IRedirectRepository redirectRepository, IActionRedirectBuilder<TTarget, TArgs> redirectBuilder, ICallValidator callValidator)
            : base(redirectRepository, redirectBuilder, callValidator)
        {
            _redirectBuilder = redirectBuilder;
        }

        public new IActionViaBuilder<TTarget, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IActionViaBuilder<TTarget, TArgs> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IActionViaBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);
            
            return this;
        }

        public IActionViaBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);
            
            return this;
        }

        public new IActionViaBuilder<TTarget, TArgs> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public new IActionCallStream<TTarget, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return base.Record(optionsAction).WithArgs<TArgs>();
        }
    }
}