using System;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class RedirectInterceptor<T> : IInterceptor where T : class
    {
        private readonly InvocationStack<T> _invocationStack;

        public RedirectInterceptor(InvocationStack<T> invocationStack)
        {
            _invocationStack = invocationStack;
        }

        public void Intercept(IInvocation invocation)
        {
            var substitutionState = _invocationStack.Peek();
            
            if (substitutionState == null)
            {
                throw new InvalidOperationException("Calls to this instance must only be made in the context of a morph invocation");
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