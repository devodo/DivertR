using System;
using Castle.DynamicProxy;

namespace Divertr
{
    internal class ReturnValueInterceptor<T> : IInterceptor where T : class
    {
        private readonly CallContext<T> _callContext;
        private readonly IInvocationCondition<T> _invocationCondition;
        private readonly object _returnValue;

        public ReturnValueInterceptor(CallContext<T> callContext, IInvocationCondition<T> invocationCondition, object returnValue)
        {
            _callContext = callContext;
            _invocationCondition = invocationCondition;
            _returnValue = returnValue;
        }

        public void Intercept(IInvocation invocation)
        {
            var substitutionState = _callContext.Peek();
            
            if (substitutionState == null)
            {
                throw new InvalidOperationException("This instance may only be accessed in the context of a Diverter Proxy call");
            }

            if (_invocationCondition.IsMatch(invocation))
            {
                invocation.ReturnValue = _returnValue;
            }
            
            var substitute = substitutionState.MoveNext(invocation) ?? substitutionState.Origin;

            if (substitute == null)
            {
                invocation.Proceed();
                return;
            }

            try
            {
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(substitute);
                invocation.Proceed();
            }
            finally
            {
                substitutionState.MoveBack();
            }
        }
    }
}