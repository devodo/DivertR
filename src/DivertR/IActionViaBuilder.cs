using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IActionViaBuilder<TTarget> where TTarget : class
    {
        IVia<TTarget> Via { get; }
        IActionRedirectBuilder<TTarget> RedirectBuilder { get; }
        IActionViaBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        
        IActionViaBuilder<TTarget> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IActionViaBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IActionViaBuilder<TTarget> Redirect(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);

        IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionViaBuilder<TTarget, TArgs> Redirect<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IActionViaBuilder<TTarget, TArgs> Retarget<TArgs>(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionViaBuilder<TTarget, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IActionCallStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IActionCallStream<TTarget, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IActionViaBuilder<TTarget, TArgs> : IActionViaBuilder<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        new IActionRedirectBuilder<TTarget, TArgs> RedirectBuilder { get; }
        
        new IActionViaBuilder<TTarget, TArgs> Redirect(Action redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IActionViaBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IActionViaBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);

        new IActionViaBuilder<TTarget, TArgs> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        new IActionCallStream<TTarget, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
