using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IFuncCallStream<TTarget, TReturn> : ICallStream<IFuncRecordedCall<TTarget, TReturn>>
        where TTarget : class
    {
        IFuncCallStream<TTarget, TReturn, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper);
        
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn>> Verify(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments> visitor);
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn>>> Verify(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, Task> visitor);
        
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IFuncCallStream<TTarget, TReturn, TArgs> : ICallStream<IFuncRecordedCall<TTarget, TReturn, TArgs>>
        where TTarget : class
    {
        IFuncCallStream<TTarget, TReturn, TNewArgs> Args<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> mapper);

        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs> visitor);
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>>> Verify(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, Task> visitor);
        
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TNewArgs>, TNewArgs> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TNewArgs>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TNewArgs>, TNewArgs, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}