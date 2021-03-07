using System.Collections.Generic;

namespace DivertR.Core.Internal
{
    internal interface IRelayState<T> where T : class
    {
        IRedirect<T>? BeginCall(T? original, List<IRedirect<T>> redirects, ICall call);
        void EndCall(ICall call);
        IRedirect<T>? BeginNextRedirect(ICall call);
        void EndRedirect(ICall invocation);
        T? Original { get; }
        IRedirect<T> Redirect { get; }
    }
}