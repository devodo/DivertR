using System;
using System.Collections;
using System.Collections.Generic;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class ActionRedirectBuilder<TTarget> : RedirectBuilder<TTarget>, IActionRedirectBuilder<TTarget> where TTarget : class?
    {
        protected readonly ICallValidator CallValidator;

        public ActionRedirectBuilder(ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(callConstraint)
        {
            CallValidator = callValidator;
        }
        
        protected ActionRedirectBuilder(ICallValidator callValidator, List<ICallConstraint<TTarget>> callConstraints)
            : base(callConstraints)
        {
            CallValidator = callValidator;
        }
        
        public new IActionRedirectBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }
        
        public IRedirect Build(Action redirectDelegate)
        {
            return Build(_ => redirectDelegate.Invoke());
        }

        public IRedirect Build(Action<IActionRedirectCall<TTarget>> redirectDelegate)
        {
            var callHandler = new ActionRedirectCallHandler<TTarget>(redirectDelegate);

            return base.Build(callHandler);
        }

        public IRedirect Build(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate)
        {
            var callHandler = new ActionArgsRedirectCallHandler<TTarget>(redirectDelegate);

            return base.Build(callHandler);
        }

        public IRedirect Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(redirectDelegate);
        }

        public IRedirect Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(redirectDelegate);
        }

        public new IActionRecordRedirect<TTarget> Record()
        {
            var recordRedirect = base.Record();
            var callStream = new ActionCallStream<TTarget>(recordRedirect.RecordStream, CallValidator);
                
            return new ActionRecordRedirect<TTarget>(recordRedirect.Redirect, callStream);
        }

        public IActionRedirectBuilder<TTarget, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ActionRedirectBuilder<TTarget, TArgs>(CallValidator, CallConstraints);
        }
    }

    internal class ActionRedirectBuilder<TTarget, TArgs> : ActionRedirectBuilder<TTarget>, IActionRedirectBuilder<TTarget, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public ActionRedirectBuilder(ICallValidator callValidator, List<ICallConstraint<TTarget>> callConstraints)
            : base(callValidator, callConstraints)
        {
            _valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(_valueTupleMapper);
        }

        public IRedirect Build(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate)
        {
            var callHandler = new ActionRedirectCallHandler<TTarget, TArgs>(_valueTupleMapper, redirectDelegate);
            
            return base.Build(callHandler);
        }

        public IRedirect Build(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate)
        {
            var callHandler = new ActionArgsRedirectCallHandler<TTarget, TArgs>(_valueTupleMapper, redirectDelegate);
            
            return base.Build(callHandler);
        }

        public new IActionRecordRedirect<TTarget, TArgs> Record()
        {
            var recordRedirect = base.Record();
            var callStream = recordRedirect.CallStream.Args<TArgs>();
                
            return new ActionRecordRedirect<TTarget, TArgs>(recordRedirect.Redirect, callStream);
        }
    }
}
