using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class RedirectRoute<T> where T : class
    {
        public CallRelay<T> CallRelay { get; }
        public List<Redirect<T>> Redirects { get; }
        
        public RedirectRoute(Redirect<T> redirect, CallRelay<T> callRelay)
        {
            Redirects = new List<Redirect<T>> {redirect};
            CallRelay = callRelay;
        }

        public RedirectRoute(List<Redirect<T>> redirects, CallRelay<T> callRelay)
        {
            Redirects = redirects;
            CallRelay = callRelay;
        }
    }
}