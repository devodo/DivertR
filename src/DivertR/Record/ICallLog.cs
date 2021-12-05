using System;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface ICallLog<out T> : ICallStream<T>, IReadOnlyCollection<T>
    {
        new ICallLog<TMap> Map<TMap>(Func<T, TMap> mapper);
    }
}