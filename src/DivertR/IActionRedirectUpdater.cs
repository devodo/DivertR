using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IActionRedirectUpdater<TTarget> where TTarget : class?
    {
        public IRedirect<TTarget> Redirect { get; }
        
        IActionRedirectUpdater<TTarget> Filter(ICallConstraint callConstraint);
        
        IActionRedirectUpdater<TTarget> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IActionRedirectUpdater<TTarget> Via(Action<IActionRedirectCall<TTarget>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);

        IActionRedirectUpdater<TTarget, TArgs> Via<TArgs>(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionRedirectUpdater<TTarget, TArgs> Via<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
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
        new IActionRedirectUpdater<TTarget, TArgs> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IActionRedirectUpdater<TTarget, TArgs> Via(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);

        new IActionRedirectUpdater<TTarget, TArgs> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        new IActionCallStream<TTarget, TArgs> Record(Action<IViaOptionsBuilder>? optionsAction = null);
    }
}
