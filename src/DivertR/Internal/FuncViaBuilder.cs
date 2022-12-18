using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class FuncViaBuilder<TTarget, TReturn> : ViaBuilder<TTarget>, IFuncViaBuilder<TTarget, TReturn> where TTarget : class?
    {
        protected readonly ICallValidator CallValidator;

        public FuncViaBuilder(ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(callConstraint)
        {
            CallValidator = callValidator;
        }
        
        protected FuncViaBuilder(ICallValidator callValidator, List<ICallConstraint<TTarget>> callConstraints)
            : base(callConstraints)
        {
            CallValidator = callValidator;
        } 
        
        public new IFuncViaBuilder<TTarget, TReturn> Filter(ICallConstraint<TTarget> callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public IVia Build(TReturn instance)
        {
            return Build(_ => instance);
        }
        
        public IVia Build(Func<TReturn> viaDelegate)
        {
            return Build(_ => viaDelegate.Invoke());
        }

        public IVia Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate)
        {
            var callHandler = new FuncCallHandler<TTarget, TReturn>(viaDelegate);
            
            return base.Build(callHandler);
        }

        public IVia Build(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> viaDelegate)
        {
            var callHandler = new FuncCallHandlerArgs<TTarget, TReturn>(viaDelegate);
            
            return base.Build(callHandler);
        }

        public IVia Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(viaDelegate);
        }

        public IVia Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> viaDelegate) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(viaDelegate);
        }

        public new IFuncRecordVia<TTarget, TReturn> Record()
        {
            var recordVia = base.Record();
            var calls = recordVia.RecordStream.Select(call => new FuncRecordedCall<TTarget, TReturn>(call));
            var callStream = new FuncCallStream<TTarget, TReturn>(calls, CallValidator);
                
            return new FuncRecordVia<TTarget, TReturn>(recordVia.Via, callStream);
        }

        public IFuncViaBuilder<TTarget, TReturn, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncViaBuilder<TTarget, TReturn, TArgs>(CallValidator, CallConstraints);
        }
    }

    internal class FuncViaBuilder<TTarget, TReturn, TArgs> : FuncViaBuilder<TTarget, TReturn>, IFuncViaBuilder<TTarget, TReturn, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public FuncViaBuilder(ICallValidator callValidator, List<ICallConstraint<TTarget>> callConstraints)
            : base(callValidator, callConstraints)
        {
            _valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(_valueTupleMapper);
        }

        public IVia Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate)
        {
            ICallHandler<TTarget> callHandler = new FuncCallHandler<TTarget, TReturn, TArgs>(_valueTupleMapper, viaDelegate);
            
            return base.Build(callHandler);
        }
        
        public IVia Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> viaDelegate)
        {
            ICallHandler<TTarget> callHandler = new FuncCallHandlerArgs<TTarget, TReturn, TArgs>(_valueTupleMapper, viaDelegate);
            
            return base.Build(callHandler);
        }

        public new IFuncRecordVia<TTarget, TReturn, TArgs> Record()
        {
            var recordVia = base.Record();
            var callStream = recordVia.CallStream.Args<TArgs>();
                
            return new FuncRecordVia<TTarget, TReturn, TArgs>(recordVia.Via, callStream);
        }
    }

    internal class FuncViaBuilder<TReturn> : ViaBuilder, IFuncViaBuilder<TReturn>
    {
        public FuncViaBuilder(ICallConstraint callConstraint) : base(callConstraint)
        {
        }

        public new IFuncViaBuilder<TReturn> Filter(ICallConstraint callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public IVia Build(TReturn instance)
        {
            return Build(_ => instance);
        }

        public IVia Build(Func<TReturn> viaDelegate)
        {
            return Build(_ => viaDelegate.Invoke());
        }

        public IVia Build(Func<IFuncRedirectCall<TReturn>, TReturn> viaDelegate)
        {
            var callHandler = new FuncCallHandler<TReturn>(viaDelegate);
            
            return base.Build(callHandler);
        }

        public IVia Build(Func<IFuncRedirectCall<TReturn>, CallArguments, TReturn> viaDelegate)
        {
            var callHandler = new FuncCallHandlerArgs<TReturn>(viaDelegate);
            
            return base.Build(callHandler);
        }

        public new IFuncRecordVia<TReturn> Record()
        {
            var recordVia = base.Record();
            var calls = recordVia.RecordStream.Select(call => new FuncRecordedCall<TReturn>(call));
            var callStream = new FuncCallStream<TReturn>(calls);
                
            return new FuncRecordVia<TReturn>(recordVia.Via, callStream);
        }
    }
}
