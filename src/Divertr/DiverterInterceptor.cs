using System;
using Castle.DynamicProxy;

namespace Divertr
{
    internal class DiversionInterceptor<T> : IInterceptor where T : class
    {
        private readonly T _origin;
        private readonly Func<Diversion<T>> _getDiversion;

        public DiversionInterceptor(T origin, Func<Diversion<T>> getDiversion)
        {
            _origin = origin;
            _getDiversion = getDiversion;
        }

        public void Intercept(IInvocation invocation)
        {
            var diversion = _getDiversion();

            if (diversion == null ||
                !diversion.TryBeginRedirectCallContext(_origin, invocation, out T redirect))
            {
                if (_origin == null)
                {
                    throw new DiverterException("Origin reference not set to an instance of an object.");
                }

                invocation.Proceed();
                return;
            }

            try
            {
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirect);
                invocation.Proceed();
            }
            finally
            {
                diversion.CloseRedirectCallContext(invocation);
            }
        }
    }
}