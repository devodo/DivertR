using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IActionRedirectBuilder<TTarget> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        new IActionRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);

        Redirect<TTarget> Build(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        Redirect<TTarget> Build(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        Redirect<TTarget> Build(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        Redirect<TTarget> Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        Redirect<TTarget> Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        new IActionRedirectBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IActionRedirectBuilder<TTarget> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IActionRedirectBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IActionRedirectBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionRedirectBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionRedirectBuilder<TTarget, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IActionCallLog<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IActionCallLog<TTarget, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IActionRedirectBuilder<TTarget, out TArgs> : IActionRedirectBuilder<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        Redirect<TTarget> Build(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        Redirect<TTarget> Build(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IActionRedirectBuilder<TTarget, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IActionRedirectBuilder<TTarget, TArgs> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IActionRedirectBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IActionRedirectBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IActionRedirectBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IActionCallLog<TTarget, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
