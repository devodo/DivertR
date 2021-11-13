using System;
using System.Collections;

namespace DivertR
{
    public interface IActionRedirectBuilder<TTarget> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        new IActionRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);

        Redirect<TTarget> Build(Action redirectDelegate);
        Redirect<TTarget> Build<T1>(Action<T1> redirectDelegate);
        Redirect<TTarget> Build<T1, T2>(Action<T1, T2> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate);
        
        IVia<TTarget> Redirect(Action redirectDelegate);
        IVia<TTarget> Redirect<T1>(Action<T1> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2>(Action<T1, T2> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate);
    }
    
    public interface IActionRedirectBuilder<TTarget, out TArgs> : IActionRedirectBuilder<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IActionRedirectBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        new IActionRedirectBuilder<TTarget, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
