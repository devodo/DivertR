using System.Collections.Generic;

namespace DivertR.Core.Internal
{
    internal interface IRelayState<T> where T : class
    {
        T? Original { get; }
        IRedirect<T> Redirect { get; }
        T Proxy { get; }
        CallInfo CallInfo { get; }
        object? CallBegin(T proxy, T? original, List<IRedirect<T>> redirects, CallInfo callInfo);
        object? CallOriginal(CallInfo callInfo);
        object? CallNext(CallInfo callInfo);
    }
}