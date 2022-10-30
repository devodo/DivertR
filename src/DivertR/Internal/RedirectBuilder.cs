using System;
using System.Collections.Generic;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class?
    {
        protected readonly List<ICallConstraint<TTarget>> CallConstraints;

        public RedirectBuilder(ICallConstraint<TTarget>? callConstraint = null)
        {
            CallConstraints = new List<ICallConstraint<TTarget>>();
            
            if (callConstraint != null)
            {
                CallConstraints.Add(callConstraint);
            }
        }

        protected RedirectBuilder(List<ICallConstraint<TTarget>> callConstraints)
        {
            CallConstraints = callConstraints;
        }

        public IRedirectBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint)
        {
            CallConstraints.Add(callConstraint);

            return this;
        }

        public IRedirect Build(object? instance)
        {
            return Build(_ => instance);
        }

        public IRedirect Build(Func<object?> redirectDelegate)
        {
            return Build(_ => redirectDelegate.Invoke());
        }

        public IRedirect Build(Func<IRedirectCall<TTarget>, object?> redirectDelegate)
        {
            var callHandler = new RedirectCallHandler<TTarget>(redirectDelegate);
            
            return Build(callHandler);
        }

        public IRedirect Build(Func<IRedirectCall<TTarget>, CallArguments, object?> redirectDelegate)
        {
            var callHandler = new RedirectArgsCallHandler<TTarget>(redirectDelegate);
            
            return Build(callHandler);
        }

        public IRedirect Build(ICallHandler<TTarget> callHandler)
        {
            return new Redirect<TTarget>(callHandler, new CompositeCallConstraint<TTarget>(CallConstraints));
        }
        
        public IRecordRedirect<TTarget> Record()
        {
            var recordHandler = new RecordCallHandler<TTarget>();

            return new RecordRedirect<TTarget>(Build(recordHandler), recordHandler.RecordStream);
        }
    }
    
    internal class RedirectBuilder : IRedirectBuilder
    {
        private readonly List<ICallConstraint> _callConstraints;

        public RedirectBuilder(ICallConstraint? callConstraint = null)
        {
            _callConstraints = new List<ICallConstraint>();
            
            if (callConstraint != null)
            {
                _callConstraints.Add(callConstraint);
            }
        }

        protected RedirectBuilder(List<ICallConstraint> callConstraints)
        {
            _callConstraints = callConstraints;
        }

        public IRedirectBuilder Filter(ICallConstraint callConstraint)
        {
            _callConstraints.Add(callConstraint);

            return this;
        }

        public IRedirect Build(object? instance)
        {
            return Build(_ => instance);
        }

        public IRedirect Build(Func<object?> redirectDelegate)
        {
            return Build(_ => redirectDelegate.Invoke());
        }

        public IRedirect Build(Func<IRedirectCall, object?> redirectDelegate)
        {
            var callHandler = new RedirectCallHandler(redirectDelegate);
            
            return Build(callHandler);
        }

        public IRedirect Build(Func<IRedirectCall, CallArguments, object?> redirectDelegate)
        {
            var callHandler = new RedirectArgsCallHandler(redirectDelegate);
            
            return Build(callHandler);
        }

        public IRedirect Build(ICallHandler callHandler)
        {
            return new Redirect(callHandler, new CompositeCallConstraint(_callConstraints));
        }

        public IRecordRedirect Record()
        {
            var recordHandler = new RecordCallHandler();

            return new RecordRedirect(Build(recordHandler), recordHandler.RecordStream);
        }
    }
}