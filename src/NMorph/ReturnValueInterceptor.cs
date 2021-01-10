using System;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class ReturnValueInterceptor<T> : IInterceptor where T : class
    {
        private readonly InvocationStack<T> _invocationStack;
        private readonly IInvocationCondition<T> _invocationCondition;
        private readonly object _returnValue;

        public ReturnValueInterceptor(InvocationStack<T> invocationStack, IInvocationCondition<T> invocationCondition, object returnValue)
        {
            _invocationStack = invocationStack;
            _invocationCondition = invocationCondition;
            _returnValue = returnValue;
        }

        public void Intercept(IInvocation invocation)
        {
            var substitutionState = _invocationStack.Peek();
            
            if (substitutionState == null)
            {
                throw new InvalidOperationException("Calls to this instance must only be made in the context of a morph invocation");
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