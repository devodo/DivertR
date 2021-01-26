using System;
using Castle.DynamicProxy;

namespace Divertr
{
    internal class DiversionInterceptor<T> : IInterceptor where T : class
    {
        private readonly T _originTarget;
        private readonly Func<Diversion<T>> _getDiversion;

        public DiversionInterceptor(T originTarget, Func<Diversion<T>> getDiversion)
        {
            _originTarget = originTarget;
            _getDiversion = getDiversion;
        }

        public void Intercept(IInvocation invocation)
        {
            var diversion = _getDiversion();
            var redirectionContext = diversion?.CreateRedirectionContext(_originTarget);
            var substitute = redirectionContext?.MoveNext(invocation);

            if (substitute == null)
            {
                invocation.Proceed();
                return;
            }
            
            diversion.CallContext.Push(redirectionContext);

            try
            {
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(substitute);
                invocation.Proceed();
            }
            finally
            {
                var poppedContext = diversion.CallContext.Pop();
                
                if (poppedContext == null)
                {
                    throw new DiverterException("Fatal error: Encountered an unexpected null redirection context");
                }

                if (!ReferenceEquals(poppedContext, redirectionContext))
                {
                    throw new DiverterException("Fatal error: Encountered an unexpected redirection context");
                }
            }
            
        }
    }
}