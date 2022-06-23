﻿using System;
using System.Collections;

namespace DivertR
{
    public interface IClassFuncViaBuilder<TTarget, TReturn> : IFuncViaBuilder<TTarget, TReturn>
        where TTarget : class
        where TReturn : class
    {
        IVia<TReturn> Divert(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IVia<TReturn> Divert(string name, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IClassFuncViaBuilder<TTarget, TReturn> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IClassFuncViaBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IClassFuncViaBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IClassFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IClassFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IClassFuncViaBuilder<TTarget, TReturn> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Retarget<TArgs>(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IClassFuncViaBuilder<TTarget, TReturn, TArgs> : IFuncViaBuilder<TTarget, TReturn, TArgs>
        where TTarget : class
        where TReturn : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IVia<TReturn> Divert(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IVia<TReturn> Divert(string name, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IClassFuncViaBuilder<TTarget, TReturn, TArgs> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
