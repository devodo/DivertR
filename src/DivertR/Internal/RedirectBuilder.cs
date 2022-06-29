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

        public IRedirect Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target);

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
        protected CompositeCallConstraint CallConstraint { get; private set; } = CompositeCallConstraint.Empty;

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

        public IRedirect Build(object target, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var callHandler = new TargetCallHandler(target);

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
