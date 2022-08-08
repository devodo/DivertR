using System;
using System.Collections;
using System.Collections.Concurrent;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class ActionRedirectBuilder<TTarget> : RedirectBuilder<TTarget>, IActionRedirectBuilder<TTarget> where TTarget : class
    {
        protected readonly ICallValidator CallValidator;

        public ActionRedirectBuilder(ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(callConstraint)
        {
            CallValidator = callValidator;
        }
        
        protected ActionRedirectBuilder(ICallValidator callValidator, ConcurrentBag<ICallConstraint<TTarget>> callConstraints)
            : base(callConstraints)
        {
            CallValidator = callValidator;
        }
        
        public new IActionRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }
        
        public IRedirect Build(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(call => redirectDelegate.Invoke(), optionsAction);
        }

        public IRedirect Build(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new ActionRedirectCallHandler<TTarget>(redirectDelegate);

            return base.Build(callHandler, optionsAction);
        }

        public IRedirect Build(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new ActionArgsRedirectCallHandler<TTarget>(redirectDelegate);

            return base.Build(callHandler, optionsAction);
        }

        public IRedirect Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public IRedirect Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public new IActionRecordRedirect<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = base.Record(optionsAction);
            var callStream = new ActionCallStream<TTarget>(recordRedirect.RecordStream, CallValidator);
                
            return new ActionRecordRedirect<TTarget>(recordRedirect.Redirect, callStream);
        }

        public IActionRedirectBuilder<TTarget, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ActionRedirectBuilder<TTarget, TArgs>(CallValidator, CallConstraints);
        }
    }

    internal class ActionRedirectBuilder<TTarget, TArgs> : ActionRedirectBuilder<TTarget>, IActionRedirectBuilder<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public ActionRedirectBuilder(ICallValidator callValidator, ConcurrentBag<ICallConstraint<TTarget>> callConstraints)
            : base(callValidator, callConstraints)
        {
            _valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(_valueTupleMapper);
        }

        public IRedirect Build(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new ActionRedirectCallHandler<TTarget, TArgs>(_valueTupleMapper, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public IRedirect Build(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new ActionArgsRedirectCallHandler<TTarget, TArgs>(_valueTupleMapper, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public new IActionRecordRedirect<TTarget, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = base.Record(optionsAction);
            var callStream = recordRedirect.CallStream.Args<TArgs>();
                
            return new ActionRecordRedirect<TTarget, TArgs>(recordRedirect.Redirect, callStream);
        }
    }
}
