using System;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class DiversionInterceptor<T> : IInterceptor where T : class
    {
        private readonly T _originTarget;
        private readonly Func<Diversion<T>> _getAlteration;

        public DiversionInterceptor(T originTarget, Func<Diversion<T>> getAlteration)
        {
            _originTarget = originTarget;
            _getAlteration = getAlteration;
        }

        public void Intercept(IInvocation invocation)
        {
            var alteration = _getAlteration();
            var substitutionState = alteration?.CreateSubstitutionState(_originTarget);
            var substitute = substitutionState?.MoveNext(invocation);

            if (substitute == null)
            {
                invocation.Proceed();
                return;
            }
            
            alteration.CallContext.InvocationStack.Push(substitutionState);

            try
            {
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(substitute);
                invocation.Proceed();
            }
            finally
            {
                var poppedContext = alteration.CallContext.InvocationStack.Pop();
                
                if (poppedContext == null)
                {
                    throw new DivertrException("Fatal error: Encountered an unexpected null invocation state");
                }

                if (!ReferenceEquals(poppedContext, substitutionState))
                {
                    throw new DivertrException("Fatal error: Encountered an unexpected invocation state");
                }
            }
            
        }
    }
}