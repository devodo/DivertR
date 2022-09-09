using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class FuncRedirectBuilder<TTarget, TReturn> : RedirectBuilder<TTarget>, IFuncRedirectBuilder<TTarget, TReturn> where TTarget : class?
    {
        protected readonly ICallValidator CallValidator;

        public FuncRedirectBuilder(ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(callConstraint)
        {
            CallValidator = callValidator;
        }
        
        protected FuncRedirectBuilder(ICallValidator callValidator, List<ICallConstraint<TTarget>> callConstraints)
            : base(callConstraints)
        {
            CallValidator = callValidator;
        } 
        
        public new IFuncRedirectBuilder<TTarget, TReturn> Filter(ICallConstraint<TTarget> callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public IRedirect Build(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(_ => instance, optionsAction);
        }
        
        public IRedirect Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(_ => redirectDelegate.Invoke(), optionsAction);
        }

        public IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new FuncRedirectCallHandler<TTarget, TReturn>(redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new FuncArgsRedirectCallHandler<TTarget, TReturn>(redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public IRedirect Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public IRedirect Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public new IFuncRecordRedirect<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = base.Record(optionsAction);
            var calls = recordRedirect.RecordStream.Select(call => new FuncRecordedCall<TTarget, TReturn>(call));
            var callStream = new FuncCallStream<TTarget, TReturn>(calls, CallValidator);
                
            return new FuncRecordRedirect<TTarget, TReturn>(recordRedirect.Redirect, callStream);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncRedirectBuilder<TTarget, TReturn, TArgs>(CallValidator, CallConstraints);
        }
    }

    internal class FuncRedirectBuilder<TTarget, TReturn, TArgs> : FuncRedirectBuilder<TTarget, TReturn>, IFuncRedirectBuilder<TTarget, TReturn, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public FuncRedirectBuilder(ICallValidator callValidator, List<ICallConstraint<TTarget>> callConstraints)
            : base(callValidator, callConstraints)
        {
            _valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(_valueTupleMapper);
        }

        public IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new FuncRedirectCallHandler<TTarget, TReturn, TArgs>(_valueTupleMapper, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }
        
        public IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new FuncArgsRedirectCallHandler<TTarget, TReturn, TArgs>(_valueTupleMapper, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public new IFuncRecordRedirect<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = base.Record(optionsAction);
            var callStream = recordRedirect.CallStream.Args<TArgs>();
                
            return new FuncRecordRedirect<TTarget, TReturn, TArgs>(recordRedirect.Redirect, callStream);
        }
    }

    internal class FuncRedirectBuilder<TReturn> : RedirectBuilder, IFuncRedirectBuilder<TReturn>
    {
        public FuncRedirectBuilder(ICallConstraint callConstraint) : base(callConstraint)
        {
        }

        public new IFuncRedirectBuilder<TReturn> Filter(ICallConstraint callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public IRedirect Build(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return Build(_ => instance, optionsAction);
        }

        public IRedirect Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return Build(_ => redirectDelegate.Invoke(), optionsAction);
        }

        public IRedirect Build(Func<IFuncRedirectCall<TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var callHandler = new FuncRedirectCallHandler<TReturn>(redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public IRedirect Build(Func<IFuncRedirectCall<TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var callHandler = new FuncArgsRedirectCallHandler<TReturn>(redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public new IFuncRecordRedirect<TReturn> Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var recordRedirect = base.Record(optionsAction);
            var calls = recordRedirect.RecordStream.Select(call => new FuncRecordedCall<TReturn>(call));
            var callStream = new FuncCallStream<TReturn>(calls);
                
            return new FuncRecordRedirect<TReturn>(recordRedirect.Redirect, callStream);
        }
    }
}
