using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IActionCallStream<TTarget> : ICallStream<IRecordedCall<TTarget>>
        where TTarget : class
    {
        IActionCallStream<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper);
        
        IVerifySnapshot<IRecordedCall<TTarget>> Verify(Action<IRecordedCall<TTarget>, CallArguments> visitor);
        Task<IVerifySnapshot<IRecordedCall<TTarget>>> Verify(Func<IRecordedCall<TTarget>, CallArguments, Task> visitor);
        
        IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>(Action<IRecordedCall<TTarget, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>(Action<IRecordedCall<TTarget, TArgs>, TArgs> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IRecordedCall<TTarget, TArgs>>> Verify<TArgs>(Func<IRecordedCall<TTarget, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IRecordedCall<TTarget, TArgs>>> Verify<TArgs>(Func<IRecordedCall<TTarget, TArgs>, TArgs, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IActionCallStream<TTarget, TArgs> : ICallStream<IRecordedCall<TTarget, TArgs>>
        where TTarget : class
    {
        IActionCallStream<TTarget, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TArgs, TMap> mapper);

        IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify(Action<IRecordedCall<TTarget, TArgs>, TArgs> visitor);
        Task<IVerifySnapshot<IRecordedCall<TTarget, TArgs>>> Verify(Func<IRecordedCall<TTarget, TArgs>, TArgs, Task> visitor);
        
        IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>(Action<IRecordedCall<TTarget, TNewArgs>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>(Action<IRecordedCall<TTarget, TNewArgs>, TNewArgs> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>> Verify<TNewArgs>(Func<IRecordedCall<TTarget, TNewArgs>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>> Verify<TNewArgs>(Func<IRecordedCall<TTarget, TNewArgs>, TNewArgs, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}