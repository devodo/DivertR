using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class RedirectRoute<T> where T : class
    {
        private readonly CallRelay<T> _callRelay;
        private readonly List<Redirect<T>> _redirects;
        
        public RedirectRoute(Redirect<T> redirect, CallRelay<T> callRelay)
        {
            _redirects = new List<Redirect<T>> {redirect};
            _callRelay = callRelay;
        }

        private RedirectRoute(List<Redirect<T>> redirects, CallRelay<T> callRelay)
        {
            _redirects = redirects;
            _callRelay = callRelay;
        }
        
        public RedirectRoute<T> AppendRedirect(Redirect<T> redirect)
        {
            var redirects = new[] {redirect}.Concat(_redirects).ToList();
            return new RedirectRoute<T>(redirects, _callRelay);
        }

        public Redirect<T>? BeginCall(T? original, IInvocation invocation)
        {
            if (_redirects.Count == 0)
            {
                return null;
            }
            
            var redirectContext = new RedirectContext<T>(original, _redirects, invocation);
            var redirect = redirectContext.BeginNextRedirect(invocation);

            if (redirect != null)
            {
                _callRelay.Push(redirectContext);
            }

            return redirect;
        }

        public void EndCall(IInvocation invocation)
        {
            var redirectContext = _callRelay.Pop();
            
            // Assert the call context is as expected
            if (redirectContext == null)
            {
                throw new DiverterException("Fatal error: Encountered an unexpected null redirect context");
            }

            if (!ReferenceEquals(invocation, redirectContext.RootInvocation))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect context");
            }
        }
    }
}