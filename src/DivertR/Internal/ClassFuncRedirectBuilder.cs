using System;

namespace DivertR.Internal
{
    internal class ClassFuncRedirectBuilder<TTarget, TReturn> : FuncRedirectBuilder<TTarget, TReturn>, IClassFuncRedirectBuilder<TTarget, TReturn>
        where TTarget : class
        where TReturn : class
    {
        public ClassFuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression) : base(via, parsedCallExpression)
        {
        }

        protected ClassFuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint, Relay<TTarget, TReturn> relay) : base(via, parsedCallExpression, callConstraint, relay)
        {
        }

        public IVia<TReturn> Divert(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var via = new Via<TReturn>();
            ICallHandler<TTarget> callHandler = new DelegateCallHandler<TTarget>(callInfo => via.Proxy((TReturn) Via.Relay.CallNext()!));
            base.InsertRedirect(callHandler, optionsAction);

            return via;
        }
    }
}