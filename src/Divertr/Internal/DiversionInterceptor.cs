using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class DiversionInterceptor<T> : IInterceptor where T : class
    {
        private readonly T? _root;
        private readonly Func<DiversionRoute<T>?> _getDiversionRoute;

        public DiversionInterceptor(T? root, Func<DiversionRoute<T>?> getDiversionRoute)
        {
            _root = root;
            _getDiversionRoute = getDiversionRoute;
        }

        public void Intercept(IInvocation invocation)
        {
            var diversionRoute = _getDiversionRoute();

            if (diversionRoute == null ||
                !diversionRoute.TryBeginCall(_root, invocation, out T? redirect))
            {
                if (_root == null)
                {
                    throw new DiverterException("Root target not set to an instance of an object.");
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
                diversionRoute.EndCall(invocation);
            }
        }
    }
}