using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IActionRedirectBuilder<TTarget> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        new IActionRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);

        IRedirect<TTarget> Build(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect<TTarget> Build(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect<TTarget> Build(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IRedirect<TTarget> Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IRedirect<TTarget> Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionRedirectBuilder<TTarget, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IActionRedirectBuilder<TTarget, out TArgs> : IActionRedirectBuilder<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IRedirect<TTarget> Build(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect<TTarget> Build(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
    
    public interface IActionRedirectBuilder : IDelegateRedirectBuilder
    {
        new IActionRedirectBuilder AddConstraint(ICallConstraint callConstraint);

        IRedirect Build(Action redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(Action<IActionRedirectCall> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(Action<IActionRedirectCall, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}
