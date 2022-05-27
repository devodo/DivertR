using System.Collections.Generic;

namespace DivertR.Record
{
    public interface IVerifySnapshot<out T> : IReadOnlyList<T>
    {
    }
}