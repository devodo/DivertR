using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DivertR
{
    public interface IFuncRedirectCall<out TReturn> : IRedirectCall
    {
        [return: MaybeNull]
        new TReturn CallNext();
        
        [return: MaybeNull]
        new TReturn CallNext(CallArguments args);
        
        [return: MaybeNull]
        new TReturn CallRoot();
        
        [return: MaybeNull]
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
