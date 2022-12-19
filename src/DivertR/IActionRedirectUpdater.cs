﻿using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IActionRedirectUpdater<TTarget> where TTarget : class?
    {
        IRedirect<TTarget> Redirect { get; }
        IActionViaBuilder<TTarget> ViaBuilder { get; }
        IActionRedirectUpdater<TTarget> Filter(ICallConstraint<TTarget> callConstraint);
        
        IActionRedirectUpdater<TTarget> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IActionRedirectUpdater<TTarget> Via(Action<IActionRedirectCall<TTarget>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IActionRedirectUpdater<TTarget> Via(Action<IActionRedirectCall<TTarget>, CallArguments> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);

        IActionRedirectUpdater<TTarget, TArgs> Via<TArgs>(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionRedirectUpdater<TTarget, TArgs> Via<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionRedirectUpdater<TTarget, TArgs> Via<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionRedirectUpdater<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        IActionRedirectUpdater<TTarget, TArgs> Retarget<TArgs>(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionRedirectUpdater<TTarget, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionCallStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null);
        
        IActionCallStream<TTarget, TArgs> Record<TArgs>(Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IActionRedirectUpdater<TTarget, TArgs> : IActionRedirectUpdater<TTarget>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        new IActionViaBuilder<TTarget, TArgs> ViaBuilder { get; }
        
        new IActionRedirectUpdater<TTarget, TArgs> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IActionRedirectUpdater<TTarget, TArgs> Via(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IActionRedirectUpdater<TTarget, TArgs> Via(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);

        new IActionRedirectUpdater<TTarget, TArgs> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        new IActionCallStream<TTarget, TArgs> Record(Action<IViaOptionsBuilder>? optionsAction = null);
    }
}