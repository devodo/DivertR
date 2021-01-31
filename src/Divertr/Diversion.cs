using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace Divertr
{
    internal class Diversion<T> where T : class
    {
        private readonly CallContext<T> _callContext;
        private readonly List<Redirect<T>> _redirects;
        
        public Diversion(Redirect<T> redirect, CallContext<T> callContext)
        {
            _redirects = new List<Redirect<T>> {redirect};
            _callContext = callContext;
        }

        private Diversion(List<Redirect<T>> redirects, CallContext<T> callContext)
        {
            _redirects = redirects;
            _callContext = callContext;
        }
        
        public Diversion<T> AppendRedirection(Redirect<T> redirect)
        {
            var substitutions = new[] {redirect}.Concat(_redirects).ToList();
            return new Diversion<T>(substitutions, _callContext);
        }

        public bool TryBeginRedirectCallContext(T origin, IInvocation invocation, out T redirect)
        {
            if (_redirects.Count == 0)
            {
                redirect = null;
                return false;
            }
            
            var redirectionContext = new RedirectionContext<T>(origin, _redirects, invocation);
            
            var hasRedirect = redirectionContext.MoveNext(invocation, out redirect);

            if (hasRedirect)
            {
                _callContext.Push(redirectionContext);
            }
            
            return hasRedirect;
        }

        public void CloseRedirectCallContext(IInvocation invocation)
        {
            var redirectContext = _callContext.Pop();
            
            // Assert the call context is as expected
            if (redirectContext == null)
            {
                throw new DiverterException("Fatal error: Encountered an unexpected null redirection context");
            }

            if (!ReferenceEquals(invocation, redirectContext.RootInvocation))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirection context");
            }
        }
    }
}