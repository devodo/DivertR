using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class FuncViaBuilder<TTarget, TReturn> : ViaBuilder, IFuncViaBuilder<TTarget, TReturn> where TTarget : class?
    {
        public ICallValidator CallValidator { get; }

        public FuncViaBuilder(ICallValidator callValidator, ICallConstraint callConstraint)
            : base(callConstraint)
        {
            CallValidator = callValidator;
        }
        
        protected FuncViaBuilder(ICallValidator callValidator, List<ICallConstraint> callConstraints)
            : base(callConstraints)
        {
            CallValidator = callValidator;
        } 
        
        public new IFuncViaBuilder<TTarget, TReturn> Filter(ICallConstraint callConstraint)
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
            var callHandler = new FuncCallHandler<TReturn>(viaDelegate);
            
            return base.Build(callHandler);
        }

        public IVia Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate)
        {
            var callHandler = new FuncRedirectCallHandler<TTarget, TReturn>(viaDelegate);
            
            return base.Build(callHandler);
        }

        public IVia Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(viaDelegate);
        }

        IFuncViaBuilder<TTarget, TReturn, TArgs> IFuncViaBuilder<TTarget, TReturn>.Args<TArgs>() => Args<TArgs>();

        public FuncViaBuilder<TTarget, TReturn, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncViaBuilder<TTarget, TReturn, TArgs>(CallValidator, CallConstraints);
        }
    }

    internal class FuncViaBuilder<TTarget, TReturn, TArgs> : FuncViaBuilder<TTarget, TReturn>, IFuncViaBuilder<TTarget, TReturn, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public FuncViaBuilder(ICallValidator callValidator, List<ICallConstraint> callConstraints)
            : base(callValidator, callConstraints)
        {
            _valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(_valueTupleMapper);
        }

        public IVia Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate)
        {
            var callHandler = new FuncRedirectCallHandler<TTarget, TReturn, TArgs>(_valueTupleMapper, viaDelegate);
            
            return base.Build(callHandler);
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
            var callHandler = new FuncRedirectCallHandler<TReturn>(viaDelegate);
            
            return base.Build(callHandler);
        }
    }
}