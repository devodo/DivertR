using System;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class ViaBuilder<TTarget> : IViaBuilder<TTarget> where TTarget : class
    {
        protected readonly IRedirectRepository RedirectRepository;

        protected CompositeCallConstraint<TTarget> CallConstraint { get; private set; } = CompositeCallConstraint<TTarget>.Empty;

        public ViaBuilder(IRedirectRepository redirectRepository, ICallConstraint<TTarget>? callConstraint = null)
        {
            RedirectRepository = redirectRepository ?? throw new ArgumentNullException(nameof(redirectRepository));

            if (callConstraint != null)
            {
                CallConstraint = CallConstraint.AddCallConstraint(callConstraint);
            }
        }

        public IViaBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            CallConstraint = CallConstraint.AddCallConstraint(callConstraint);

            return this;
        }

        public IRedirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target);

            return Build(callHandler, optionsAction);
        }

        public IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(target, optionsAction);
            
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>();
            var redirect = Build(recordHandler, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return recordHandler.RecordStream;
        }

        protected IRedirect<TTarget> Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(callHandler, optionsAction.Create());
        }

        private IRedirect<TTarget> Build(ICallHandler<TTarget> callHandler, IRedirectOptions<TTarget> redirectOptions)
        {
            return new Redirect<TTarget>(callHandler, CallConstraint, redirectOptions);
        }
    }
}
