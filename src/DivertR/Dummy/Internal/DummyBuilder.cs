using System;

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
        
        public IDummyBuilder<TReturn> AddConstraint(ICallConstraint callConstraint)
        {
            _redirectBuilder.AddConstraint(callConstraint);

            return this;
        }

        public IDummyBuilder<TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(instance, optionsAction);
            _redirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IDummyBuilder<TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            _redirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IDummyBuilder<TReturn> Redirect(Func<IFuncRedirectCall<TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            _redirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IDummyBuilder<TReturn> Redirect(Func<IFuncRedirectCall<TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            _redirectRepository.InsertRedirect(redirect);

            return this;
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
        
        public IDummyBuilder AddConstraint(ICallConstraint callConstraint)
        {
            _redirectBuilder.AddConstraint(callConstraint);

            return this;
        }

        public IDummyBuilder Redirect(object instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(instance, optionsAction);
            _redirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IDummyBuilder Redirect(Func<object> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            _redirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IDummyBuilder Redirect(Func<IRedirectCall, object> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            _redirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IDummyBuilder Redirect(Func<IRedirectCall, CallArguments, object> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            _redirectRepository.InsertRedirect(redirect);

            return this;
        }
    }
}