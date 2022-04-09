using System;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        protected readonly IVia<TTarget> Via;

        protected CompositeCallConstraint CallConstraint { get; private set; } = CompositeCallConstraint.Empty;

        public RedirectBuilder(IVia<TTarget> via, ICallConstraint? callConstraint = null)
        {
            Via = via ?? throw new ArgumentNullException(nameof(via));

            if (callConstraint != null)
            {
                CallConstraint = CallConstraint.AddCallConstraint(callConstraint);
            }
        }

        public IRedirectBuilder<TTarget> AddConstraint(ICallConstraint callConstraint)
        {
            CallConstraint = CallConstraint.AddCallConstraint(callConstraint);

            return this;
        }

        public Redirect Build(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            ICallHandler callHandler = new TargetCallHandler<TTarget>(target);

            return Build(callHandler, optionsAction);
        }

        public IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = Build(target, optionsAction);
            
            return Via.InsertRedirect(redirect);
        }

        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>();
            var redirect = Build(recordHandler, optionsAction);
            Via.InsertRedirect(redirect);

            return recordHandler.RecordStream;
        }

        protected Redirect Build(ICallHandler callHandler, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            return Build(callHandler, optionsAction.Create());
        }

        private Redirect Build(ICallHandler callHandler, RedirectOptions redirectOptions)
        {
            return new Redirect(callHandler, CallConstraint, redirectOptions);
        }
    }
}
