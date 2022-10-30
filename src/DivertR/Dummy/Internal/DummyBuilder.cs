using System;
using DivertR.Internal;
using DivertR.Record;

namespace DivertR.Dummy.Internal
{
    internal class DummyBuilder<TReturn> : IDummyBuilder<TReturn>
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IFuncRedirectBuilder<TReturn> _redirectBuilder;

        public DummyBuilder(IRedirectRepository redirectRepository, IFuncRedirectBuilder<TReturn> redirectBuilder)
        {
            _redirectRepository = redirectRepository;
            _redirectBuilder = redirectBuilder;
        }
        
        public IDummyBuilder<TReturn> Filter(ICallConstraint callConstraint)
        {
            _redirectBuilder.Filter(callConstraint);

            return this;
        }

        public IDummyBuilder<TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(instance);
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(redirect, options);

            return this;
        }

        public IDummyBuilder<TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate);
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(redirect, options);

            return this;
        }

        public IDummyBuilder<TReturn> Redirect(Func<IFuncRedirectCall<TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate);
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(redirect, options);

            return this;
        }

        public IDummyBuilder<TReturn> Redirect(Func<IFuncRedirectCall<TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate);
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(redirect, options);

            return this;
        }
        
        public IFuncCallStream<TReturn> Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var recordRedirect = _redirectBuilder.Record();
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(recordRedirect.Redirect, options);

            return recordRedirect.CallStream;
        }
    }
    
    internal class DummyBuilder : IDummyBuilder
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IRedirectBuilder _redirectBuilder;

        public DummyBuilder(IRedirectRepository redirectRepository, IRedirectBuilder redirectBuilder)
        {
            _redirectRepository = redirectRepository;
            _redirectBuilder = redirectBuilder;
        }
        
        public IDummyBuilder Filter(ICallConstraint callConstraint)
        {
            _redirectBuilder.Filter(callConstraint);

            return this;
        }

        public IDummyBuilder Redirect(object instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(instance);
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(redirect, options);

            return this;
        }

        public IDummyBuilder Redirect(Func<object> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate);
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(redirect, options);

            return this;
        }

        public IDummyBuilder Redirect(Func<IRedirectCall, object> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate);
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(redirect, options);

            return this;
        }

        public IDummyBuilder Redirect(Func<IRedirectCall, CallArguments, object> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate);
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(redirect, options);

            return this;
        }
        
        public IRecordStream Record(Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var recordRedirect = _redirectBuilder.Record();
            var options = RedirectOptionsBuilder.Create(optionsAction);
            _redirectRepository.InsertRedirect(recordRedirect.Redirect, options);

            return recordRedirect.RecordStream;
        }
    }
}