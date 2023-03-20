using System;
using System.Collections;

namespace DivertR
{
    public interface IFuncViaBuilder<TTarget, TReturn> : IViaBuilder where TTarget : class?
    {
        new IFuncViaBuilder<TTarget, TReturn> Filter(ICallConstraint callConstraint);

        IVia Build(TReturn instance);
        IVia Build(Func<TReturn> viaDelegate);
        IVia Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate);

        IVia Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncViaBuilder<TTarget, TReturn, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IFuncViaBuilder<TTarget, TReturn, out TArgs> : IFuncViaBuilder<TTarget, TReturn>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IVia Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate);
    }

    public interface IFuncViaBuilder<TReturn> : IViaBuilder
    {
        new IFuncViaBuilder<TReturn> Filter(ICallConstraint callConstraint);
        
        IVia Build(TReturn instance);
        IVia Build(Func<TReturn> viaDelegate);
        IVia Build(Func<IFuncRedirectCall<TReturn>, TReturn> viaDelegate);
    }
}