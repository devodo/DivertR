using System.Collections.Generic;

namespace DivertR.Core.Internal
{
    internal interface IViaState<T> where T : class
    {
        IRelayState<T> RelayState { get; }
        List<IRedirect<T>> Redirects { get; }
    }
}