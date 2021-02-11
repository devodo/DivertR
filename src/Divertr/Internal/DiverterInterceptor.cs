using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class DiverterInterceptor<T> : IInterceptor where T : class
    {
        private readonly T? _original;
        private readonly Func<CallRoute<T>?> _getDirector;

        public DiverterInterceptor(T? original, Func<CallRoute<T>?> getDirector)
        {
            _original = original;
            _getDirector = getDirector;
        }

        public void Intercept(IInvocation invocation)
        {
            var director = _getDirector();

            if (director == null ||
                !director.TryBeginCallContext(_original, invocation, out T? redirect))
            {
                if (_original == null)
                {
                    throw new DiverterException("Original reference not set to an instance of an object.");
                }

                invocation.Proceed();
                return;
            }

            try
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirect);
                invocation.Proceed();
            }
            finally
            {
                director.CloseCallContext(invocation);
            }
        }
    }
}