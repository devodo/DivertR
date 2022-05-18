﻿using System;
using System.Collections;

namespace DivertR
{
    public interface IActionRedirectCall : IRedirectCall
    {
        new void CallNext();
        new void CallNext(CallArguments args);
        new void CallRoot();
        new void CallRoot(CallArguments args);
    }
    
    public interface IActionRedirectCall<TTarget> : IRedirectCall, IRedirectCall<TTarget> where TTarget : class
    {
    }
    
    public interface IActionRedirectCall<TTarget, out TArgs> : IActionRedirectCall<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        new TArgs Args { get; }
    }
}
