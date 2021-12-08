using System;
using System.Collections;

namespace DivertR
{
    public interface IActionRedirectCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        CallArguments Args { get; }
        IRelay<TTarget> Relay { get; }
        TTarget Next { get; }
        TTarget Root { get; }
    }
    
    public interface IActionRedirectCall<TTarget, out TArgs> : IActionRedirectCall<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        new TArgs Args { get; }
    }
}