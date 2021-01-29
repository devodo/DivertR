using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace Divertr
{
    internal class Diversion<T> where T : class
    {
        private readonly CallContext<T> _callContext;
        private readonly List<Redirection<T>> _redirections;
        
        public Diversion(Redirection<T> redirection, CallContext<T> callContext)
        {
            _redirections = new List<Redirection<T>> {redirection};
            _callContext = callContext;
        }

        private Diversion(List<Redirection<T>> redirections, CallContext<T> callContext)
        {
            _redirections = redirections;
            _callContext = callContext;
        }
        
        public Diversion<T> AppendRedirection(Redirection<T> redirection)
        {
            var substitutions = new[] {redirection}.Concat(_redirections).ToList();
            return new Diversion<T>(substitutions, _callContext);
        }

        public bool TryBeginRedirectCallContext(T origin, IInvocation invocation, out T redirect)
        {
            if (_redirections.Count == 0)
            {
                redirect = null;
                return false;
            }
            
            var redirectionContext = new RedirectionContext<T>(origin, _redirections, invocation);
            
            var hasRedirect = redirectionContext.MoveNext(invocation, out redirect);

            if (hasRedirect)
            {
                _callContext.Push(redirectionContext);
            }
            
            return hasRedirect;
        }

        public void CloseRedirectCallContext(IInvocation invocation)
        {
            var redirectionContext = _callContext.Pop();
            
            // Assert the call context is as expected
            if (redirectionContext == null)
            {
                throw new DiverterException("Fatal error: Encountered an unexpected null redirection context");
            }

            if (!ReferenceEquals(invocation, redirectionContext.RootInvocation))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirection context");
            }
        }
    }
}