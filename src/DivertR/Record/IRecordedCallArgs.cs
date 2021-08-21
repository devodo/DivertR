using System;

namespace DivertR.Record
{
    public interface IRecordedCallArgs
    {
        T1 Args<T1>(Action<T1>? action = null);
        (T1, T2) Args<T1, T2>(Action<T1, T2>? action = null);
        (T1, T2, T3) Args<T1, T2, T3>(Action<T1, T2, T3>? action = null);
        (T1, T2, T3, T4) Args<T1, T2, T3, T4>(Action<T1, T2, T3, T4>? action = null);
        (T1, T2, T3, T4, T5) Args<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5>? action = null);
        (T1, T2, T3, T4, T5, T6) Args<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6>? action = null);
        (T1, T2, T3, T4, T5, T6, T7) Args<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7>? action = null);
        (T1, T2, T3, T4, T5, T6, T7, T8) Args<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8>? action = null);
    }
}