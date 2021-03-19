using System.Collections.Generic;

namespace DivertR.Core.Internal
{
    internal interface IRelayState<T> where T : class
    {
        object? CallBegin(T? original, List<IRedirect<T>> redirects, ICall call);
        object? InvokeNext(ICall call);
        T? Original { get; }
        IRedirect<T> Redirect { get; }
    }
}