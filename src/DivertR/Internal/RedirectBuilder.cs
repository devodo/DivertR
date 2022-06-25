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

        public IRedirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target);

            return Build(callHandler, optionsAction);
        }
        
        public IRedirect<TTarget> Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(callHandler, optionsAction.Create());
        }

        public IRedirect<TTarget> Build(ICallHandler<TTarget> callHandler, IRedirectOptions<TTarget> redirectOptions)
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
}
