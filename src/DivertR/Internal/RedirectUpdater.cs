using System;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectUpdater<TTarget> : IRedirectUpdater<TTarget> where TTarget : class?
    {
        private readonly ViaBuilder<TTarget> _viaBuilder;
        public IRedirect<TTarget> Redirect { get; }
        
        public RedirectUpdater(IRedirect<TTarget> redirect, ViaBuilder<TTarget> viaBuilder)
        {
            Redirect = redirect;
            _viaBuilder = viaBuilder;
        }
        
        public IRedirectUpdater<TTarget> Filter(ICallConstraint<TTarget> callConstraint)
        {
            _viaBuilder.Filter(callConstraint);

            return this;
        }

        public IRedirectUpdater<TTarget> Via(Func<object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectUpdater<TTarget> Via(Func<IRedirectCall<TTarget>, object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectUpdater<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target, Redirect.RedirectSet.Settings.CallInvoker);
            var via = _viaBuilder.Build(callHandler);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRecordStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>();
            var recordVia = _viaBuilder.Build(recordHandler);
            
            InsertVia(recordVia, optionsAction, disableSatisfyStrict: true);

            return recordHandler.RecordStream;
        }

        protected void InsertVia(IVia via, Action<IViaOptionsBuilder>? optionsAction, bool disableSatisfyStrict = false)
        {
            var options = ViaOptionsBuilder.Create(optionsAction, disableSatisfyStrict: disableSatisfyStrict);
            Redirect.RedirectRepository.InsertVia(via, options);
        }
    }
}
