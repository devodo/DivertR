using System;
using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IVerifySnapshot<out T> : IReadOnlyList<T>
    {
        IVerifySnapshot<T> ForEach(Action<T> visitor);
        
        IVerifySnapshot<T> ForEach(Action<T, int> visitor);
    }
}