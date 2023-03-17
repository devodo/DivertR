using System;
using System.Collections;

namespace DivertR
{
    public interface IActionRedirectCall<TTarget> : IRedirectCall<TTarget> where TTarget : class?
    {
    }
    
    public interface IActionRedirectCall<TTarget, out TArgs> : IActionRedirectCall<TTarget>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        new TArgs Args { get; }
    }
}
