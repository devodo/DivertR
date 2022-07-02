﻿using System;
using System.Collections;
using System.Linq;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class FuncRedirectBuilder<TTarget, TReturn> : DelegateRedirectBuilder<TTarget>, IFuncRedirectBuilder<TTarget, TReturn> where TTarget : class
    {
        public FuncRedirectBuilder(ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(callValidator, callConstraint)
        {
        }
        
        public new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }

        public IRedirect Build(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(call => instance, optionsAction);
        }
        
        public IRedirect Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(call => redirectDelegate.Invoke(), optionsAction);
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
            return WithArgs<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public IRedirect Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public new IFuncRecordRedirect<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = base.Record(optionsAction);
            var calls = recordRedirect.RecordStream.Select(call => new FuncRecordedCall<TTarget, TReturn>(call));
            var callStream = new FuncCallStream<TTarget, TReturn>(calls, CallValidator);
                
            return new FuncRecordRedirect<TTarget, TReturn>(recordRedirect.Redirect, callStream);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncRedirectBuilder<TTarget, TReturn, TArgs>(CallValidator, CallConstraint);
        }
    }

    internal class FuncRedirectBuilder<TTarget, TReturn, TArgs> : FuncRedirectBuilder<TTarget, TReturn>, IFuncRedirectBuilder<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public FuncRedirectBuilder(ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(callValidator, callConstraint)
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
            var callStream = recordRedirect.CallStream.WithArgs<TArgs>();
                
            return new FuncRecordRedirect<TTarget, TReturn, TArgs>(recordRedirect.Redirect, callStream);
        }
    }

    internal class FuncRedirectBuilder<TReturn> : DelegateRedirectBuilder, IFuncRedirectBuilder<TReturn>
    {
        public FuncRedirectBuilder(ICallValidator callValidator, ICallConstraint callConstraint)
            : base(callValidator, callConstraint)
        {
        }

        public new IFuncRedirectBuilder<TReturn> AddConstraint(ICallConstraint callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }

        public IRedirect Build(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return Build(call => instance, optionsAction);
        }

        public IRedirect Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return Build(call => redirectDelegate.Invoke(), optionsAction);
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
    }
}
