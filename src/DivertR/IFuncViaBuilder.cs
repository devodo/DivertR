using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IFuncViaBuilder<TTarget, TReturn> : IViaBuilder<TTarget> where TTarget : class?
    {
        new IFuncViaBuilder<TTarget, TReturn> Filter(ICallConstraint<TTarget> callConstraint);

        IVia Build(TReturn instance);
        IVia Build(Func<TReturn> viaDelegate);
        IVia Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate);
        IVia Build(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> viaDelegate);
        
        IVia Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IVia Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> viaDelegate)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        new IFuncRecordVia<TTarget, TReturn> Record();

        IFuncViaBuilder<TTarget, TReturn, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IFuncViaBuilder<TTarget, TReturn, TArgs> : IFuncViaBuilder<TTarget, TReturn>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IVia Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate);
        IVia Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> viaDelegate);
        new IFuncRecordVia<TTarget, TReturn, TArgs> Record();
    }

    public interface IFuncViaBuilder<TReturn> : IViaBuilder
    {
        new IFuncViaBuilder<TReturn> Filter(ICallConstraint callConstraint);
        
        IVia Build(TReturn instance);
        IVia Build(Func<TReturn> viaDelegate);
        IVia Build(Func<IFuncRedirectCall<TReturn>, TReturn> viaDelegate);
        IVia Build(Func<IFuncRedirectCall<TReturn>, CallArguments, TReturn> viaDelegate);
        new IFuncRecordVia<TReturn> Record();
    }
}
