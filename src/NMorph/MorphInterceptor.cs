using System;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class MorphInterceptor<T> : IInterceptor where T : class
    {
        private readonly T _originTarget;
        private readonly Func<Alteration<T>> _getAlteration;

        public MorphInterceptor(T originTarget, Func<Alteration<T>> getAlteration)
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
            
            alteration.InvocationStack.Push(substitutionState);

            try
            {
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(substitute);
                invocation.Proceed();
            }
            finally
            {
                var poppedContext = alteration.InvocationStack.Pop();
                
                if (poppedContext == null)
                {
                    throw new MorphException("Fatal error: Encountered an unexpected null invocation state");
                }

                if (!ReferenceEquals(poppedContext, substitutionState))
                {
                    throw new MorphException("Fatal error: Encountered an unexpected invocation state");
                }
            }
            
        }
    }
}