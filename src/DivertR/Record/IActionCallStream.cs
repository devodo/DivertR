using System;
using System.Collections;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface IActionCallStream<TTarget> : ICallStream<IRecordedCall<TTarget>>
        where TTarget : class?
    {
        IActionCallStream<TTarget, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;

        IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>(Action<IRecordedCall<TTarget, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IRecordedCall<TTarget, TArgs>>> Verify<TArgs>(Func<IRecordedCall<TTarget, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    public interface IActionCallStream<TTarget, TArgs> : ICallStream<IRecordedCall<TTarget, TArgs>>
        where TTarget : class?
    {
        IActionCallStream<TTarget, TNewArgs> Args<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>(Action<IRecordedCall<TTarget, TNewArgs>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        Task<IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>> Verify<TNewArgs>(Func<IRecordedCall<TTarget, TNewArgs>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
}