using System;
using DivertR.Internal;
using DivertR.Record;

namespace DivertR.Dummy.Internal
{
    internal class DummyBuilder<TReturn> : IDummyBuilder<TReturn>
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IFuncViaBuilder<TReturn> _viaBuilder;

        public DummyBuilder(IRedirectRepository redirectRepository, IFuncViaBuilder<TReturn> viaBuilder)
        {
            _redirectRepository = redirectRepository;
            _viaBuilder = viaBuilder;
        }
        
        public IDummyBuilder<TReturn> Filter(ICallConstraint callConstraint)
        {
            _viaBuilder.Filter(callConstraint);

            return this;
        }

        public IDummyBuilder<TReturn> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(instance);
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(via, options);

            return this;
        }

        public IDummyBuilder<TReturn> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(via, options);

            return this;
        }

        public IDummyBuilder<TReturn> Via(Func<IFuncRedirectCall<TReturn>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(via, options);

            return this;
        }

        public IDummyBuilder<TReturn> Via(Func<IFuncRedirectCall<TReturn>, CallArguments, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(via, options);

            return this;
        }
        
        public IFuncCallStream<TReturn> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordVia = _viaBuilder.Record();
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(recordVia.Via, options);

            return recordVia.CallStream;
        }
    }
    
    internal class DummyBuilder : IDummyBuilder
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IViaBuilder _viaBuilder;

        public DummyBuilder(IRedirectRepository redirectRepository, IViaBuilder viaBuilder)
        {
            _redirectRepository = redirectRepository;
            _viaBuilder = viaBuilder;
        }
        
        public IDummyBuilder Filter(ICallConstraint callConstraint)
        {
            _viaBuilder.Filter(callConstraint);

            return this;
        }

        public IDummyBuilder Via(object instance, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(instance);
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(via, options);

            return this;
        }

        public IDummyBuilder Via(Func<object> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(via, options);

            return this;
        }

        public IDummyBuilder Via(Func<IRedirectCall, object> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(via, options);

            return this;
        }

        public IDummyBuilder Via(Func<IRedirectCall, CallArguments, object> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(via, options);

            return this;
        }
        
        public IRecordStream Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordVia = _viaBuilder.Record();
            var options = ViaOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertVia(recordVia.Via, options);

            return recordVia.RecordStream;
        }
    }
}