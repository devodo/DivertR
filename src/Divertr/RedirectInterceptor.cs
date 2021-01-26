using System;
using Castle.DynamicProxy;

namespace Divertr
{
    internal class RedirectInterceptor<T> : IInterceptor where T : class
    {
        private readonly CallContext<T> _callContext;

        public RedirectInterceptor(CallContext<T> callContext)
        {
            _callContext = callContext;
        }

        public void Intercept(IInvocation invocation)
        {
            var substitutionState = _callContext.Peek();
            
            if (substitutionState == null)
            {
                throw new InvalidOperationException("This instance may only be accessed in the context of a Diverter Proxy call");
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