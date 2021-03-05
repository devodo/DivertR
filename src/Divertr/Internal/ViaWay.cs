using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class ViaWay<T> where T : class
    {
        public Relay<T> Relay { get; }
        public List<Redirect<T>> Redirects { get; }
        
        public ViaWay(Redirect<T> redirect, Relay<T> relay)
        {
            Redirects = new List<Redirect<T>> {redirect};
            Relay = relay;
        }

        public ViaWay(List<Redirect<T>> redirects, Relay<T> relay)
        {
            Redirects = redirects;
            Relay = relay;
        }
    }
}