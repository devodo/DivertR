using System;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectUpdater<TTarget> : IRedirectUpdater<TTarget> where TTarget : class?
    {
        public ViaBuilder ViaBuilder { get; }
        public Redirect<TTarget> Redirect { get; }
        
        public RedirectUpdater(Redirect<TTarget> redirect, ViaBuilder viaBuilder)
        {
            Redirect = redirect;
            ViaBuilder = viaBuilder;
        }
        
        public IRedirectUpdater<TTarget> Filter(ICallConstraint callConstraint)
        {
            ViaBuilder.Filter(callConstraint);

            return this;
        }
        
        public IRedirectUpdater<TTarget> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = ViaBuilder.Build(callHandler);
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
        
        public IRedirectUpdater<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var callHandler = new TargetCallHandler<TTarget>(target, Redirect.RedirectSet.Settings.CallInvoker);
            var via = ViaBuilder.Build(callHandler);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRecordStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>();
            var recordVia = ViaBuilder.Build(recordHandler);
            
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
