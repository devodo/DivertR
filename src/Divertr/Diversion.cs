using System.Collections.Generic;
using System.Linq;

namespace Divertr
{
    internal class Diversion<T> where T : class
    {
        private readonly List<Redirection<T>> _redirections;

        public CallContext<T> CallContext { get; }
        
        public Diversion(CallContext<T> callContext)
        {
            _redirections = new List<Redirection<T>>();
            CallContext = callContext;
        }

        public Diversion(Redirection<T> redirection, CallContext<T> callContext)
        {
            _redirections = new List<Redirection<T>> {redirection};
            CallContext = callContext;
        }

        private Diversion(List<Redirection<T>> redirections, CallContext<T> callContext)
        {
            _redirections = redirections;
            CallContext = callContext;
        }

        public RedirectionContext<T> CreateRedirectionContext(T origin)
        {
            return _redirections.Count == 0 ? null : new RedirectionContext<T>(origin, _redirections);
        }

        public Diversion<T> AppendRedirection(Redirection<T> redirection)
        {
            var substitutions = new[] {redirection}.Concat(_redirections).ToList();
            return new Diversion<T>(substitutions, CallContext);
        }
        
        public Diversion<T> Reset()
        {
            return new Diversion<T>(CallContext);
        }
    }
}