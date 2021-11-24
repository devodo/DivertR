using System;
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
            
            return Via.InsertRedirect(redirect);
        }

        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>(Via.Relay);
            var redirectOptions = optionsAction.Create(Via);

            if (CallConstraint == CompositeCallConstraint<TTarget>.Empty)
            {
                redirectOptions.DisableSatisfyStrict ??= true;
            }
            
            InsertRedirect(recordHandler, redirectOptions);

            return recordHandler.RecordStream;
        }

        public ISpyCollection<TMap> Spy<TMap>(Func<IRecordedCall<TTarget>, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var spyCallHandler = new SpyCallHandler<TTarget, TMap>(Via.Relay, mapper);
            InsertRedirect(spyCallHandler, optionsAction.Create(Via));

            return spyCallHandler.MappedCalls;
        }

        public ISpyCollection<TMap> Spy<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var spyMapper = new SpyArgsActionMapper<TTarget, TMap>(mapper);

            return Spy(spyMapper.Map, optionsAction);
        }

        protected Redirect<TTarget> Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(callHandler, optionsAction.Create(Via));
        }

        private Redirect<TTarget> Build(ICallHandler<TTarget> callHandler, RedirectOptions<TTarget> redirectOptions)
        {
            return new Redirect<TTarget>(callHandler, CallConstraint, redirectOptions);
        }

        private void InsertRedirect(ICallHandler<TTarget> callHandler, RedirectOptions<TTarget> redirectOptions)
        {
            var redirect = Build(callHandler, redirectOptions);
            Via.InsertRedirect(redirect);
        }
    }
}
