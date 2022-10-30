using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    public interface IFuncRedirectBuilder<TTarget, TReturn> : IRedirectBuilder<TTarget> where TTarget : class?
    {
        new IFuncRedirectBuilder<TTarget, TReturn> Filter(ICallConstraint<TTarget> callConstraint);

        IRedirect Build(TReturn instance);
        IRedirect Build(Func<TReturn> redirectDelegate);
        IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate);
        IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate);
        
        IRedirect Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IRedirect Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        new IFuncRecordRedirect<TTarget, TReturn> Record();

        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }

    public interface IFuncRedirectBuilder<TTarget, TReturn, TArgs> : IFuncRedirectBuilder<TTarget, TReturn>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate);
        IRedirect Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate);
        new IFuncRecordRedirect<TTarget, TReturn, TArgs> Record();
    }

    public interface IFuncRedirectBuilder<TReturn> : IRedirectBuilder
    {
        new IFuncRedirectBuilder<TReturn> Filter(ICallConstraint callConstraint);
        
        IRedirect Build(TReturn instance);
        IRedirect Build(Func<TReturn> redirectDelegate);
        IRedirect Build(Func<IFuncRedirectCall<TReturn>, TReturn> redirectDelegate);
        IRedirect Build(Func<IFuncRedirectCall<TReturn>, CallArguments, TReturn> redirectDelegate);
        new IFuncRecordRedirect<TReturn> Record();
    }
}
