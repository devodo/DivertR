using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface ICallStream<T> : IReadOnlyCollection<T>
    {
        ICallStream<T> Filter(Func<T, bool> predicate);
        ICallStream<TMap> Map<TMap>(Func<T, TMap> mapper);
        IVerifySnapshot<T> Verify();
        IVerifySnapshot<T> Verify(Action<T> visitor);
        Task<IVerifySnapshot<T>> Verify(Func<T, Task> visitor);
    }
}