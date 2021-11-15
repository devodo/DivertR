using System;
using System.Collections.Generic;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        protected readonly IVia<TTarget> Via;

        protected CompositeCallConstraint<TTarget> CallConstraint { get; private set; } = CompositeCallConstraint<TTarget>.Empty;

        public RedirectBuilder(IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
        {
            Via = via ?? throw new ArgumentNullException(nameof(via));

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

        public Redirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target);

            return Build(callHandler, optionsAction);
        }

        public IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(target, optionsAction);
            
            return InsertRedirect(redirect);
        }

        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>(Via.Relay);
            var redirectOptions = BuildOptions(optionsAction);

            if (CallConstraint == CompositeCallConstraint<TTarget>.Empty)
            {
                redirectOptions.DisableSatisfyStrict ??= true;
            }
            
            InsertRedirect(recordHandler, redirectOptions);

            return recordHandler.RecordStream;
        }

        public IReadOnlyCollection<TMap> Spy<TMap>(Func<IRecordedCall<TTarget>, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var spyCallHandler = new SpyCallHandler<TTarget, TMap>(Via.Relay, mapper);
            InsertRedirect(spyCallHandler, BuildOptions(optionsAction));

            return spyCallHandler.MappedCalls;
        }

        protected Redirect<TTarget> Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirectOption = BuildOptions(optionsAction);

            return Build(callHandler, redirectOption);
        }

        protected IVia<TTarget> InsertRedirect(Redirect<TTarget> redirect)
        {
            return Via.InsertRedirect(redirect);
        }
        
        private static RedirectOptions<TTarget> BuildOptions(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var builder = new RedirectOptionsBuilder<TTarget>();
            optionsAction?.Invoke(builder);
            
            return builder.Build();
        }
        
        private Redirect<TTarget> Build(ICallHandler<TTarget> callHandler, RedirectOptions<TTarget> redirectOptions)
        {
            callHandler = redirectOptions.CallHandlerDecorator?.Invoke(Via, callHandler) ?? callHandler;

            return new Redirect<TTarget>(callHandler, CallConstraint, redirectOptions.OrderWeight, redirectOptions.DisableSatisfyStrict);
        }

        private IVia<TTarget> InsertRedirect(ICallHandler<TTarget> callHandler, RedirectOptions<TTarget> redirectOptions)
        {
            var redirect = Build(callHandler, redirectOptions);

            return InsertRedirect(redirect);
        }
    }
}
