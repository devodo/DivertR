using System.Collections.Generic;

namespace DivertR.Core.Internal
{
    internal interface IRelayState<T> where T : class
    {
        T? Original { get; }
        IRedirect<T> Redirect { get; }
        object? CallBegin(T? original, List<IRedirect<T>> redirects, ICall call);
        object? CallNext(ICall call);
    }
}