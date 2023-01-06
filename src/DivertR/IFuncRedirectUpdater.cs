using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IFuncRedirectUpdater<TTarget, TReturn> where TTarget : class?
    {
        IFuncRedirectUpdater<TTarget, TReturn> Filter(ICallConstraint<TTarget> callConstraint);
        
        IFuncRedirectUpdater<TTarget, TReturn> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null);
        IFuncRedirectUpdater<TTarget, TReturn> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IFuncRedirectUpdater<TTarget, TReturn> Via(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);

        IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via<TArgs>(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via<TArgs>(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncRedirectUpdater<TTarget, TReturn> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        IFuncRedirectUpdater<TTarget, TReturn, TArgs> Retarget<TArgs>(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncRedirectUpdater<TTarget, TReturn, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncCallStream<TTarget, TReturn> Record(Action<IViaOptionsBuilder>? optionsAction = null);
        
        IFuncCallStream<TTarget, TReturn, TArgs> Record<TArgs>(Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IFuncRedirectUpdater<TTarget, TReturn, TArgs> : IFuncRedirectUpdater<TTarget, TReturn>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        new IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null);
        new IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);

        IFuncRedirectUpdater<TTarget, TReturn, TArgs> Via(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);

        new IFuncRedirectUpdater<TTarget, TReturn, TArgs> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
       
        new IFuncCallStream<TTarget, TReturn, TArgs> Record(Action<IViaOptionsBuilder>? optionsAction = null);
    }
}
