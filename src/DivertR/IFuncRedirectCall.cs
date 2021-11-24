using System;
using System.Collections;

namespace DivertR
{
    public interface IFuncRedirectCall<TTarget, out TReturn> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        CallArguments Args { get; }
        IRelay<TTarget, TReturn> Relay { get; }
        TTarget Next { get; }
        TTarget Root { get; }
    }
    
    public interface IFuncRedirectCall<TTarget, out TReturn, out TArgs> : IFuncRedirectCall<TTarget, TReturn>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        new TArgs Args { get; }
    }
}
