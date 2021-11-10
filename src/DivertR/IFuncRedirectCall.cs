using System;
using System.Collections;

namespace DivertR
{
    public interface IFuncRedirectCall<TTarget, out TReturn> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        CallArguments Args { get; }
        IRelay<TTarget, TReturn> Relay { get; }
    }
    
    public interface IFuncRedirectCall<TTarget, out TReturn, out TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        CallInfo<TTarget> CallInfo { get; }
        TArgs Args { get; }
        IRelay<TTarget, TReturn> Relay { get; }
    }
}
