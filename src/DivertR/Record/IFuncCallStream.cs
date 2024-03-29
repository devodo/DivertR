﻿using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IFuncCallStream<TTarget, TReturn> : ICallStream<IFuncRecordedCall<TTarget, TReturn>>
        where TTarget : class?
    {
        IFuncCallStream<TTarget, TReturn, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn>>> Verify(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, Task> visitor);
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IFuncCallStream<TTarget, TReturn, TArgs> : ICallStream<IFuncRecordedCall<TTarget, TReturn, TArgs>>
        where TTarget : class?
    {
        IFuncCallStream<TTarget, TReturn, TNewArgs> Args<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TNewArgs>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IFuncCallStream<TReturn> : ICallStream<IFuncRecordedCall<TReturn>>
    {
    }
}