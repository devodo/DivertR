using System;
using System.Linq;
using DivertR.Internal;
using DivertR.Record;
using DivertR.Record.Internal;

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
            var recordHandler = new RecordCallHandler();
            var recordVia = _viaBuilder.Build(recordHandler);
            var options = ViaOptionsBuilder.Create(optionsAction, disableSatisfyStrict: true);
            _redirectRepository.InsertVia(recordVia, options);
            
            var calls = recordHandler.RecordStream.Select(call => new FuncRecordedCall<TReturn>(call));
            var callStream = new FuncCallStream<TReturn>(calls);
            
            return callStream;
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
            var recordHandler = new RecordCallHandler();
            var recordVia = _viaBuilder.Build(recordHandler);
            var options = ViaOptionsBuilder.Create(optionsAction, disableSatisfyStrict: true);
            _redirectRepository.InsertVia(recordVia, options);

            return recordHandler.RecordStream;
        }
    }
}