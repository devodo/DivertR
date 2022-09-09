using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IFuncRedirectBuilder<TTarget, TReturn> : IRedirectBuilder<TTarget> where TTarget : class?
    {
        new IFuncRedirectBuilder<TTarget, TReturn> Filter(ICallConstraint<TTarget> callConstraint);

        IRedirect Build(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IRedirect Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IRedirect Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        new IFuncRecordRedirect<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);

        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IFuncRedirectBuilder<TTarget, TReturn, TArgs> : IFuncRedirectBuilder<TTarget, TReturn>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IFuncRecordRedirect<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }

    public interface IFuncRedirectBuilder<TReturn> : IRedirectBuilder
    {
        new IFuncRedirectBuilder<TReturn> Filter(ICallConstraint callConstraint);
        
        IRedirect Build(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(Func<IFuncRedirectCall<TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(Func<IFuncRedirectCall<TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        new IFuncRecordRedirect<TReturn> Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}
