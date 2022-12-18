using System;
using DivertR.Record;

namespace DivertR.Internal
{
    internal class RedirectUpdater<TTarget> : IRedirectUpdater<TTarget> where TTarget : class?
    {
        public RedirectUpdater(IRedirect<TTarget> redirect, IViaBuilder<TTarget> redirectBuilder)
        {
            Redirect = redirect;
            ViaBuilder = redirectBuilder;
        }

        public IRedirect<TTarget> Redirect { get; }
        public IViaBuilder<TTarget> ViaBuilder { get; }

        public IRedirectUpdater<TTarget> Filter(ICallConstraint<TTarget> callConstraint)
        {
            ViaBuilder.Filter(callConstraint);

            return this;
        }

        public IRedirectUpdater<TTarget> Via(object? instance, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(instance);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectUpdater<TTarget> Via(Func<object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectUpdater<TTarget> Via(Func<IRedirectCall<TTarget>, object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectUpdater<TTarget> Via(Func<IRedirectCall<TTarget>, CallArguments, object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectUpdater<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target, Redirect.RedirectSet.Settings.CallInvoker);
            var via = ViaBuilder.Build(callHandler);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRecordStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordVia = ViaBuilder.Record();
            InsertVia(recordVia.Via, optionsAction, disableSatisfyStrict: true);

            return recordVia.RecordStream;
        }

        protected void InsertVia(IVia via, Action<IViaOptionsBuilder>? optionsAction, bool disableSatisfyStrict = false)
        {
            var options = ViaOptionsBuilder.Create(optionsAction, disableSatisfyStrict: disableSatisfyStrict);
            Redirect.RedirectRepository.InsertVia(via, options);
        }
    }
}
