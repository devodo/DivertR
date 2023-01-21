using System;
using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class ViaBuilder<TTarget> : IViaBuilder<TTarget> where TTarget : class?
    {
        protected readonly List<ICallConstraint<TTarget>> CallConstraints;

        public ViaBuilder(ICallConstraint<TTarget>? callConstraint = null)
        {
            CallConstraints = new List<ICallConstraint<TTarget>>();
            
            if (callConstraint != null)
            {
                CallConstraints.Add(callConstraint);
            }
        }

        protected ViaBuilder(List<ICallConstraint<TTarget>> callConstraints)
        {
            CallConstraints = callConstraints;
        }

        public IViaBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint)
        {
            CallConstraints.Add(callConstraint);

            return this;
        }

        public IVia Build(object? instance)
        {
            return Build(_ => instance);
        }

        public IVia Build(Func<object?> viaDelegate)
        {
            return Build(_ => viaDelegate.Invoke());
        }

        public IVia Build(Func<IRedirectCall<TTarget>, object?> viaDelegate)
        {
            var callHandler = new CallHandler<TTarget>(viaDelegate);
            
            return Build(callHandler);
        }

        public IVia Build(Func<IRedirectCall<TTarget>, CallArguments, object?> viaDelegate)
        {
            var callHandler = new CallHandlerArgs<TTarget>(viaDelegate);
            
            return Build(callHandler);
        }

        public IVia Build(ICallHandler<TTarget> callHandler)
        {
            return new Via<TTarget>(callHandler, new CompositeCallConstraint<TTarget>(CallConstraints));
        }
    }
    
    internal class ViaBuilder : IViaBuilder
    {
        private readonly List<ICallConstraint> _callConstraints;

        public ViaBuilder(ICallConstraint? callConstraint = null)
        {
            _callConstraints = new List<ICallConstraint>();
            
            if (callConstraint != null)
            {
                _callConstraints.Add(callConstraint);
            }
        }

        public IViaBuilder Filter(ICallConstraint callConstraint)
        {
            _callConstraints.Add(callConstraint);

            return this;
        }

        public IVia Build(object? instance)
        {
            return Build(_ => instance);
        }

        public IVia Build(Func<object?> viaDelegate)
        {
            return Build(_ => viaDelegate.Invoke());
        }

        public IVia Build(Func<IRedirectCall, object?> viaDelegate)
        {
            var callHandler = new CallHandler(viaDelegate);
            
            return Build(callHandler);
        }

        public IVia Build(Func<IRedirectCall, CallArguments, object?> viaDelegate)
        {
            var callHandler = new CallHandlerArgs(viaDelegate);
            
            return Build(callHandler);
        }

        public IVia Build(ICallHandler callHandler)
        {
            return new Via(callHandler, new CompositeCallConstraint(_callConstraints));
        }
    }
}