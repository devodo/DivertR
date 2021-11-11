using System;
using System.Collections;

namespace DivertR
{
    public interface IRedirectCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        CallArguments Args { get; }
        Redirect<TTarget> Redirect { get; }
    }
}