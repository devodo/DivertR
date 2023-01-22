using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class ActionViaBuilder<TTarget> : ViaBuilder<TTarget>, IActionViaBuilder<TTarget> where TTarget : class?
    {
        public ICallValidator CallValidator { get; }

        public ActionViaBuilder(ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(callConstraint)
        {
            CallValidator = callValidator;
        }
        
        protected ActionViaBuilder(ICallValidator callValidator, List<ICallConstraint<TTarget>> callConstraints)
            : base(callConstraints)
        {
            CallValidator = callValidator;
        }
        
        public new IActionViaBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }
        
        public IVia Build(Action viaDelegate)
        {
            return Build(_ => viaDelegate.Invoke());
        }

        public IVia Build(Action<IActionRedirectCall<TTarget>> viaDelegate)
        {
            var callHandler = new ActionCallHandler<TTarget>(viaDelegate);

            return base.Build(callHandler);
        }
        
        public IVia Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(viaDelegate);
        }

        public IVia Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> viaDelegate) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Build(viaDelegate);
        }

        IActionViaBuilder<TTarget, TArgs> IActionViaBuilder<TTarget>.Args<TArgs>() => Args<TArgs>();
        
        public ActionViaBuilder<TTarget, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ActionViaBuilder<TTarget, TArgs>(CallValidator, CallConstraints);
        }
    }

    internal class ActionViaBuilder<TTarget, TArgs> : ActionViaBuilder<TTarget>, IActionViaBuilder<TTarget, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public ActionViaBuilder(ICallValidator callValidator, List<ICallConstraint<TTarget>> callConstraints)
            : base(callValidator, callConstraints)
        {
            _valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(_valueTupleMapper);
        }

        public IVia Build(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate)
        {
            var callHandler = new ActionCallHandler<TTarget, TArgs>(_valueTupleMapper, viaDelegate);
            
            return base.Build(callHandler);
        }
    }
}