using System.Collections.Generic;

namespace DivertR.Core.Internal
{
    internal interface IRelayState<T> where T : class
    {
        IRedirect<T> Redirect { get; }
        CallInfo<T> CallInfo { get; }
        object? CallBegin(List<IRedirect<T>> redirects, CallInfo<T> callInfo);
        object? CallOriginal(CallInfo<T> callInfo);
        object? CallNext(CallInfo<T> callInfo);
    }
}