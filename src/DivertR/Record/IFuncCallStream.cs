using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IFuncCallStream<TTarget, TReturn> : ICallStream<IFuncRecordedCall<TTarget, TReturn>>
        where TTarget : class
    {
        IFuncCallStream<TTarget, TArgs, TReturn> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper);
        
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn>> Verify(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments> visitor);
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn>>> Verify(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, Task> visitor);
        
        IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TArgs, TReturn>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IFuncCallStream<TTarget, TArgs, TReturn> : ICallStream<IFuncRecordedCall<TTarget, TArgs, TReturn>>
        where TTarget : class
    {
        IFuncCallStream<TTarget, TNewArgs, TReturn> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs, TMap> mapper);

        IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>> Verify(Action<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs> visitor);
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>>> Verify(Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs, Task> visitor);
        
        IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TNewArgs, TReturn>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TNewArgs, TReturn>, TNewArgs> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TNewArgs, TReturn>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TNewArgs, TReturn>, TNewArgs, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}