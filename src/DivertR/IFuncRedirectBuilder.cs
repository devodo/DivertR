using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IFuncRedirectBuilder<TTarget, TReturn> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint);

        Redirect<TTarget> Build(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        Redirect<TTarget> Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        Redirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        Redirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        Redirect<TTarget> Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        Redirect<TTarget> Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IFuncRedirectBuilder<TTarget, TReturn> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncRedirectBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        new IFuncRecordCollection<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IFuncRecordCollection<TTarget, TReturn, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ISpyCollection<TMap> Spy<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        ISpyCollection<TMap> Spy<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }

    public interface IFuncRedirectBuilder<TTarget, TReturn, out TArgs> : IFuncRedirectBuilder<TTarget, TReturn>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        Redirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        Redirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);

        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
       
        new IFuncRecordCollection<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        ISpyCollection<TMap> Spy<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        ISpyCollection<TMap> Spy<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
