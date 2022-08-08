using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IActionRedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        new IActionRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);

        IRedirect Build(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IRedirect Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IRedirect Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IActionRecordRedirect<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);

        IActionRedirectBuilder<TTarget, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IActionRedirectBuilder<TTarget, TArgs> : IActionRedirectBuilder<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IRedirect Build(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IActionRecordRedirect<TTarget, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
