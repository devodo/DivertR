using System;

namespace DivertR.Internal
{
    internal class DivertRedirectBuilder<TTarget, TReturn> : IDivertRedirectBuilder<TTarget, TReturn>
        where TTarget : class
        where TReturn : class
    {
        private readonly CompositeCallConstraint<TTarget> _callConstraint = CompositeCallConstraint<TTarget>.Empty;
        private readonly IVia<TTarget> _via;
        private readonly ParsedCallExpression _parsedCallExpression;

        public DivertRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
        {
            _via = via;
            _parsedCallExpression = parsedCallExpression;
        }
        
        
        public IDivertRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            _callConstraint.AddCallConstraint(callConstraint);

            return this;
        }

        public IVia<TReturn> Via(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var via = new Via<TReturn>();
            ICallHandler<TTarget> callHandler = new DelegateCallHandler<TTarget>(callInfo => via.Proxy((TReturn) _via.Relay.CallNext()!));
            var options = optionsAction.Create();
            callHandler = options.CallHandlerDecorator?.Invoke(_via, callHandler) ?? callHandler;
            var redirect = new Redirect<TTarget>(callHandler, _parsedCallExpression.ToCallConstraint<TTarget>(), options.OrderWeight, options.DisableSatisfyStrict);
            _via.InsertRedirect(redirect);

            return via;
        }
    }
}