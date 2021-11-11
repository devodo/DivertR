using System;
using System.Collections;

namespace DivertR
{
    public interface IActionRedirectCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        CallArguments Args { get; }
        IRelay<TTarget> Relay { get; }
    }
    
    public interface IActionRedirectCall<TTarget, out TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        CallInfo<TTarget> CallInfo { get; }
        TArgs Args { get; }
        IRelay<TTarget> Relay { get; }
    }
}