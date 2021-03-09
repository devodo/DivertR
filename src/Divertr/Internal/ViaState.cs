using System.Collections.Generic;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class ViaState<T> : IViaState<T> where T : class
    {
        public IRelayState<T> RelayState { get; }
        public List<IRedirect<T>> Redirects { get; }
        
        public ViaState(IRedirect<T> redirect, IRelayState<T> relayState)
        {
            Redirects = new List<IRedirect<T>> {redirect};
            RelayState = relayState;
        }

        public ViaState(List<IRedirect<T>> redirects, IRelayState<T> relayState)
        {
            Redirects = redirects;
            RelayState = relayState;
        }
    }
}