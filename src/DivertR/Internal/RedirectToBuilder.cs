using System;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectToBuilder<TTarget> : IRedirectToBuilder<TTarget> where TTarget : class?
    {
        private readonly ViaBuilder _viaBuilder;

        public RedirectToBuilder(Redirect<TTarget> redirect, ViaBuilder viaBuilder)
        {
            Redirect = redirect;
            _viaBuilder = viaBuilder;
        }
        
        public Redirect<TTarget> Redirect { get; }
        
        public IRedirectToBuilder<TTarget> Filter(ICallConstraint callConstraint)
        {
            _viaBuilder.Filter(callConstraint);

            return this;
        }
        
        public IRedirectToBuilder<TTarget> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(callHandler);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectToBuilder<TTarget> Via(Func<object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectToBuilder<TTarget> Via(Func<IRedirectCall<TTarget>, object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }
        
        public IRedirectToBuilder<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var callHandler = new TargetCallHandler<TTarget>(target, Redirect.RedirectSet.Settings.CallInvoker);
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
