using System;
using System.Collections;
using System.Collections.Generic;
using DivertR.Record;

namespace DivertR
{
    public interface IFuncRedirectBuilder<TTarget, TReturn> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint);
        
        Redirect<TTarget> Build(TReturn instance);
        Redirect<TTarget> Build(Func<TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1>(Func<T1, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2>(Func<T1, T2, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate);
        Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate);
        
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IFuncRedirectBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IFuncRedirectBuilder<TTarget, TReturn, ValueTuple<CallArguments, T1, T2>> Redirect<T1, T2>(Func<T1, T2, TReturn> redirectDelegate);
        
        IVia<TTarget> Redirect<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate);
        IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate);

        IReadOnlyCollection<T> Record<T>(Func<IFuncRecordedCall<TTarget, TReturn>, T> visitor);

        IFuncCallStream<TTarget, TReturn> Record();
    }

    public interface IFuncRedirectBuilder<TTarget, TReturn, TArgs> : IFuncRedirectBuilder<TTarget, TReturn>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
