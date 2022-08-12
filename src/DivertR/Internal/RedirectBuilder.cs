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

        public IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            CallConstraints.Add(callConstraint);

            return this;
        }

        public IRedirect Build(object? instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(_ => instance, optionsAction);
        }

        public IRedirect Build(Func<object?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(_ => redirectDelegate.Invoke(), optionsAction);
        }

        public IRedirect Build(Func<IRedirectCall<TTarget>, object?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new RedirectCallHandler<TTarget>(redirectDelegate);
            
            return Build(callHandler, optionsAction);
        }

        public IRedirect Build(Func<IRedirectCall<TTarget>, CallArguments, object?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new RedirectArgsCallHandler<TTarget>(redirectDelegate);
            
            return Build(callHandler, optionsAction);
        }

        public IRedirect Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return RedirectOptionsBuilder<TTarget>
                .Create(optionsAction)
                .BuildRedirect(callHandler, new CompositeCallConstraint<TTarget>(CallConstraints));
        }

        public IRedirect Build(ICallHandler<TTarget> callHandler, IRedirectOptions redirectOptions)
        {
            return new Redirect<TTarget>(callHandler, new CompositeCallConstraint<TTarget>(CallConstraints), redirectOptions);
        }
        
        public IRecordRedirect<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>();
            var redirect = RedirectOptionsBuilder<TTarget>
                .Create(optionsAction, disableSatisfyStrict: true)
                .BuildRedirect(recordHandler, new CompositeCallConstraint<TTarget>(CallConstraints));

            return new RecordRedirect<TTarget>(redirect, recordHandler.RecordStream);
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

        public IRedirectBuilder AddConstraint(ICallConstraint callConstraint)
        {
            _callConstraints.Add(callConstraint);

            return this;
        }

        public IRedirect Build(object? instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return Build(_ => instance, optionsAction);
        }

        public IRedirect Build(Func<object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return Build(_ => redirectDelegate.Invoke(), optionsAction);
        }

        public IRedirect Build(Func<IRedirectCall, object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var callHandler = new RedirectCallHandler(redirectDelegate);
            
            return Build(callHandler, optionsAction);
        }

        public IRedirect Build(Func<IRedirectCall, CallArguments, object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var callHandler = new RedirectArgsCallHandler(redirectDelegate);
            
            return Build(callHandler, optionsAction);
        }

        public IRedirect Build(ICallHandler callHandler, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return RedirectOptionsBuilder
                .Create(optionsAction)
                .BuildRedirect(callHandler, new CompositeCallConstraint(_callConstraints));
        }

        public IRedirect Build(ICallHandler callHandler, IRedirectOptions redirectOptions)
        {
            return new Redirect(callHandler, new CompositeCallConstraint(_callConstraints), redirectOptions);
        }
        
        public IRecordRedirect Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler();
            var redirect = RedirectOptionsBuilder
                .Create(optionsAction, disableSatisfyStrict: true)
                .BuildRedirect(recordHandler, new CompositeCallConstraint(_callConstraints));

            return new RecordRedirect(redirect, recordHandler.RecordStream);
        }
    }
}