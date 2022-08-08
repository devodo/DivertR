using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IFuncViaBuilder<TTarget, TReturn> where TTarget : class
    {
        IVia<TTarget> Via { get; }
        IFuncRedirectBuilder<TTarget, TReturn> RedirectBuilder { get; }
        IFuncViaBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint);
        
        IFuncViaBuilder<TTarget, TReturn> Redirect(TReturn? instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncViaBuilder<TTarget, TReturn> Redirect(Func<TReturn?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncViaBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);

        IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(TReturn? instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TReturn?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncViaBuilder<TTarget, TReturn> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IFuncViaBuilder<TTarget, TReturn, TArgs> Retarget<TArgs>(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncViaBuilder<TTarget, TReturn, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncCallStream<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IFuncCallStream<TTarget, TReturn, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IFuncViaBuilder<TTarget, TReturn, TArgs> : IFuncViaBuilder<TTarget, TReturn>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        new IFuncRedirectBuilder<TTarget, TReturn, TArgs> RedirectBuilder { get; }
        
        new IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(TReturn? instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<TReturn?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);

        IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncViaBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IFuncViaBuilder<TTarget, TReturn, TArgs> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
       
        new IFuncCallStream<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
