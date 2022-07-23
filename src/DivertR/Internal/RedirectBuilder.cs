using System;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        protected CompositeCallConstraint<TTarget> CallConstraint { get; private set; } = CompositeCallConstraint<TTarget>.Empty;

        public RedirectBuilder(ICallConstraint<TTarget>? callConstraint = null)
        {
            if (callConstraint != null)
            {
                CallConstraint = CallConstraint.AddCallConstraint(callConstraint);
            }
        }

        public IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            CallConstraint = CallConstraint.AddCallConstraint(callConstraint);

            return this;
        }

        public IRedirect Build(object? instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(call => instance, optionsAction);
        }

        public IRedirect Build(Func<object?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(call => redirectDelegate.Invoke(), optionsAction);
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
            var builder = new RedirectOptionsBuilder<TTarget>();
            optionsAction?.Invoke(builder);
            
            var redirectOptions = builder.BuildOptions();
            callHandler = builder.BuildCallHandler(callHandler);
            var callConstraint = builder.BuildCallConstraint(CallConstraint);
                
            return new Redirect<TTarget>(callHandler, callConstraint, redirectOptions);
        }

        public IRedirect Build(ICallHandler<TTarget> callHandler, IRedirectOptions redirectOptions)
        {
            return new Redirect<TTarget>(callHandler, CallConstraint, redirectOptions);
        }
        
        public IRecordRedirect<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>();
            var redirect = Build(recordHandler, optionsAction);

            return new RecordRedirect<TTarget>(redirect, recordHandler.RecordStream);
        }
    }
    
    internal class RedirectBuilder : IRedirectBuilder
    {
        private CompositeCallConstraint CallConstraint { get; set; } = CompositeCallConstraint.Empty;

        public RedirectBuilder(ICallConstraint? callConstraint = null)
        {
            if (callConstraint != null)
            {
                CallConstraint = CallConstraint.AddCallConstraint(callConstraint);
            }
        }

        public IRedirectBuilder AddConstraint(ICallConstraint callConstraint)
        {
            CallConstraint = CallConstraint.AddCallConstraint(callConstraint);

            return this;
        }

        public IRedirect Build(object? instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return Build(call => instance, optionsAction);
        }

        public IRedirect Build(Func<object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return Build(call => redirectDelegate.Invoke(), optionsAction);
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
            var builder = new RedirectOptionsBuilder();
            optionsAction?.Invoke(builder);

            return Build(callHandler, builder.BuildOptions());
        }

        public IRedirect Build(ICallHandler callHandler, IRedirectOptions redirectOptions)
        {
            return new Redirect(callHandler, CallConstraint, redirectOptions);
        }
        
        public IRecordRedirect Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler();
            var redirect = Build(recordHandler, optionsAction);

            return new RecordRedirect(redirect, recordHandler.RecordStream);
        }
    }
}
