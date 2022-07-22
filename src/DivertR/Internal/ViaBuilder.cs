using System;
using DivertR.Record;

namespace DivertR.Internal
{
    internal class ViaBuilder<TTarget> : IViaBuilder<TTarget> where TTarget : class
    {
        public ViaBuilder(IVia<TTarget> via, IRedirectBuilder<TTarget> redirectBuilder)
        {
            Via = via;
            RedirectBuilder = redirectBuilder;
        }

        public IVia<TTarget> Via { get; }
        public IRedirectBuilder<TTarget> RedirectBuilder { get; }

        public IViaBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            RedirectBuilder.AddConstraint(callConstraint);

            return this;
        }
        
        public IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(target, optionsAction);
            Via.RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordRedirect = RedirectBuilder.Record(optionsAction);
            Via.RedirectRepository.InsertRedirect(recordRedirect.Redirect);

            return recordRedirect.RecordStream;
        }
    }
}
