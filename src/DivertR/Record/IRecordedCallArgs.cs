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
        
        TOut Args<T1, TOut>(Func<T1, TOut> action);
        TOut Args<T1, T2, TOut>(Func<T1, T2, TOut> action);
        TOut Args<T1, T2, T3, TOut>(Func<T1, T2, T3, TOut> action);
        TOut Args<T1, T2, T3, T4, TOut>(Func<T1, T2, T3, T4, TOut> action);
        TOut Args<T1, T2, T3, T4, T5, TOut>(Func<T1, T2, T3, T4, T5, TOut> action);
        TOut Args<T1, T2, T3, T4, T5, T6, TOut>(Func<T1, T2, T3, T4, T5, T6, TOut> action);
        TOut Args<T1, T2, T3, T4, T5, T6, T7, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, TOut> action);
        TOut Args<T1, T2, T3, T4, T5, T6, T7, T8, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TOut> action);
    }
}