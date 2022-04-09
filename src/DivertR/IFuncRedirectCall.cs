using System;
using System.Collections;

namespace DivertR
{
    public interface IFuncRedirectCall<out TReturn> : IRedirectCall
    {
        new TReturn CallNext();
        new TReturn CallNext(CallArguments args);
        new TReturn CallRoot();
        new TReturn CallRoot(CallArguments args);
    }
    
    public interface IFuncRedirectCall<TTarget, out TReturn> : IRedirectCall<TTarget>, IFuncRedirectCall<TReturn> where TTarget : class
    {
    }

    public interface IFuncRedirectCall<TTarget, out TReturn, out TArgs> : IFuncRedirectCall<TTarget, TReturn>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        new TArgs Args { get; }
    }
}
