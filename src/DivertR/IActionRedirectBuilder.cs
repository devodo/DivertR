using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IActionRedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class?
    {
        new IActionRedirectBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint);

        IRedirect Build(Action redirectDelegate);
        IRedirect Build(Action<IActionRedirectCall<TTarget>> redirectDelegate);
        IRedirect Build(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate);
        
        IRedirect Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IRedirect Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        new IActionRecordRedirect<TTarget> Record();

        IActionRedirectBuilder<TTarget, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IActionRedirectBuilder<TTarget, TArgs> : IActionRedirectBuilder<TTarget>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IRedirect Build(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate);
        IRedirect Build(Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate);
        
        new IActionRecordRedirect<TTarget, TArgs> Record();
    }
}
