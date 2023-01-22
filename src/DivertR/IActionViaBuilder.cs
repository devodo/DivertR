using System;
using System.Collections;

namespace DivertR
{
    public interface IActionViaBuilder<TTarget> : IViaBuilder<TTarget> where TTarget : class?
    {
        new IActionViaBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint);

        IVia Build(Action viaDelegate);
        IVia Build(Action<IActionRedirectCall<TTarget>> viaDelegate);

        IVia Build<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IActionViaBuilder<TTarget, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IActionViaBuilder<TTarget, out TArgs> : IActionViaBuilder<TTarget>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IVia Build(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate);
    }
}