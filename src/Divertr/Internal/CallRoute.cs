using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class CallRoute<T> where T : class
    {
        private readonly CallContext<T> _callContext;
        private readonly List<Redirect<T>> _redirects;
        
        public CallRoute(Redirect<T> redirect, CallContext<T> callContext)
        {
            _redirects = new List<Redirect<T>> {redirect};
            _callContext = callContext;
        }

        private CallRoute(List<Redirect<T>> redirects, CallContext<T> callContext)
        {
            _redirects = redirects;
            _callContext = callContext;
        }
        
        public CallRoute<T> AppendRedirect(Redirect<T> redirect)
        {
            var substitutions = new[] {redirect}.Concat(_redirects).ToList();
            return new CallRoute<T>(substitutions, _callContext);
        }

        public bool TryBeginCall(T? origin, IInvocation invocation, out T? redirect)
        {
            if (_redirects.Count == 0)
            {
                redirect = null;
                return false;
            }
            
            var redirectContext = new RedirectContext<T>(origin, _redirects, invocation);
            
            var hasRedirect = redirectContext.MoveNext(invocation, out redirect);

            if (hasRedirect)
            {
                _callContext.Push(redirectContext);
            }
            
            return hasRedirect;
        }

        public void EndCall(IInvocation invocation)
        {
            var redirectContext = _callContext.Pop();
            
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