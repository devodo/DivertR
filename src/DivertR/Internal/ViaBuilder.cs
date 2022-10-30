using System;
using DivertR.Record;

namespace DivertR.Internal
{
    internal class ViaBuilder<TTarget> : IViaBuilder<TTarget> where TTarget : class?
    {
        public ViaBuilder(IVia<TTarget> via, IRedirectBuilder<TTarget> redirectBuilder)
        {
            Via = via;
            RedirectBuilder = redirectBuilder;
        }

        public IVia<TTarget> Via { get; }
        public IRedirectBuilder<TTarget> RedirectBuilder { get; }

        public IViaBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint)
        {
            RedirectBuilder.Filter(callConstraint);

            return this;
        }

        public IViaBuilder<TTarget> Redirect(object? instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(instance);
            InsertRedirect(redirect, optionsAction);

            return this;
        }

        public IViaBuilder<TTarget> Redirect(Func<object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate);
            InsertRedirect(redirect, optionsAction);

            return this;
        }

        public IViaBuilder<TTarget> Redirect(Func<IRedirectCall<TTarget>, object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate);
            InsertRedirect(redirect, optionsAction);

            return this;
        }

        public IViaBuilder<TTarget> Redirect(Func<IRedirectCall<TTarget>, CallArguments, object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate);
            InsertRedirect(redirect, optionsAction);

            return this;
        }

        public IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target, Via.ViaSet.Settings.CallInvoker);
            var redirect = RedirectBuilder.Build(callHandler);
            InsertRedirect(redirect, optionsAction);

            return this;
        }

        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var recordRedirect = RedirectBuilder.Record();
            InsertRedirect(recordRedirect.Redirect, optionsAction, disableSatisfyStrict: true);

            return recordRedirect.RecordStream;
        }

        protected void InsertRedirect(IRedirect redirect, Action<IRedirectOptionsBuilder>? optionsAction, bool disableSatisfyStrict = false)
        {
            var options = RedirectOptionsBuilder.Create(optionsAction, disableSatisfyStrict: disableSatisfyStrict);
            Via.RedirectRepository.InsertRedirect(redirect, options);
        }
    }
}
