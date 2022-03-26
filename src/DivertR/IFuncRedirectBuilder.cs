using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IFuncRedirectBuilder<TTarget, TReturn> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint callConstraint);

        Redirect Build(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null);
        Redirect Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        Redirect Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        Redirect Build(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        
        Redirect Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        Redirect Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IFuncRedirectBuilder<TTarget, TReturn> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncRedirectBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        new IFuncCallLog<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
        
        IFuncCallLog<TTarget, TReturn, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IFuncRedirectBuilder<TTarget, TReturn, out TArgs> : IFuncRedirectBuilder<TTarget, TReturn>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        Redirect Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        Redirect Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        
        new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null);
        new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);

        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
       
        new IFuncCallLog<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}
