using System;
using DivertR.Record;

namespace DivertR.Internal
{
    internal class ViaBuilder<TTarget> : IViaBuilder<TTarget> where TTarget : class
    {
        protected readonly IRedirectRepository RedirectRepository;
        private readonly IRedirectBuilder<TTarget> _redirectBuilder;

        public ViaBuilder(IRedirectRepository redirectRepository, IRedirectBuilder<TTarget> redirectBuilder)
        {
            RedirectRepository = redirectRepository ?? throw new ArgumentNullException(nameof(redirectRepository));
            _redirectBuilder = redirectBuilder;
        }

        public IViaBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            _redirectBuilder.AddConstraint(callConstraint);

            return this;
        }
        
        public IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(target, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = _redirectBuilder.Record(optionsAction);
            RedirectRepository.InsertRedirect(recordRedirect.Redirect);

            return recordRedirect.RecordStream;
        }
    }
}
