using System;

namespace DivertR.Record
{
    public interface IRecordedCallArgs
    {
        T1 Args<T1>(Action<T1>? action = null);
        (T1, T2) Args<T1, T2>(Action<T1, T2>? action = null);
    }
}