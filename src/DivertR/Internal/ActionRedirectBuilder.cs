using System;
using System.Collections;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class ActionRedirectBuilder<TTarget> : DelegateRedirectBuilder<TTarget>, IActionRedirectBuilder<TTarget> where TTarget : class
    {
        public ActionRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression, parsedCallExpression.ToCallConstraint<TTarget>())
        {
        }
        
        protected ActionRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint)
            : base(via, parsedCallExpression, callConstraint)
        {
        }
        
        public new IActionRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }
        
        public Redirect<TTarget> Build(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ParsedCallExpression.Validate(redirectDelegate);
            var callHandler = new DelegateCallHandler<TTarget>(callInfo =>
            {
                redirectDelegate.Invoke();

                return default;
            });

            return base.Build(callHandler, optionsAction);
        }

        public Redirect<TTarget> Build(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new ActionRedirectCallHandler<TTarget>(Via.Relay, redirectDelegate);

            return base.Build(callHandler, optionsAction);
        }

        public Redirect<TTarget> Build(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new ActionArgsRedirectCallHandler<TTarget>(Via.Relay, redirectDelegate);

            return base.Build(callHandler, optionsAction);
        }

        public Redirect<TTarget> Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public Redirect<TTarget> Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public new IActionRedirectBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public IActionRedirectBuilder<TTarget> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);

            return this;
        }

        public IActionRedirectBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);
            
            return this;
        }

        public IActionRedirectBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);
            
            return this;
        }

        public IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IActionRedirectBuilder<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ActionRedirectBuilder<TTarget, TArgs>(Via, ParsedCallExpression, CallConstraint);
        }

        public new IActionCallLog<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);

            return new ActionCallLog<TTarget>(recordStream);
        }

        public IActionCallLog<TTarget, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Record(optionsAction);
        }
    }

    internal class ActionRedirectBuilder<TTarget, TArgs> : ActionRedirectBuilder<TTarget>, IActionRedirectBuilder<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public ActionRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint)
            : base(via, parsedCallExpression, callConstraint)
        {
            _valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            ParsedCallExpression.Validate(typeof(void), _valueTupleMapper.ArgumentTypes);
        }

        public Redirect<TTarget> Build(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new ActionRedirectCallHandler<TTarget, TArgs>(_valueTupleMapper, Via.Relay, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public Redirect<TTarget> Build(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new ActionArgsRedirectCallHandler<TTarget, TArgs>(_valueTupleMapper, Via.Relay, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public new IActionRedirectBuilder<TTarget, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IActionRedirectBuilder<TTarget, TArgs> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IActionRedirectBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public IActionRedirectBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);
            
            return this;
        }

        public IActionRedirectBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);
            
            return this;
        }

        public new IActionCallLog<TTarget, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);
            var mappedCollection = new MappedCollection<IRecordedCall<TTarget>, IRecordedCall<TTarget, TArgs>>(recordStream,
                call => new RecordedCall<TTarget, TArgs>(call, (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs)));

            return new ActionCallLog<TTarget, TArgs>(mappedCollection);
        }
    }
}