using System;
using System.Collections.Generic;
using System.Linq;

namespace DivertR.Record
{
    public static class CallStreamExtensions
    {
        public static IEnumerable<TRecordedCall> ForEach<TRecordedCall>(this IEnumerable<TRecordedCall> recordedCalls, Action<TRecordedCall> visitor)
            where TRecordedCall : IRecordedCall
        {
            return recordedCalls.Select(call =>
            {
                visitor.Invoke(call);

                return call;
            });
        }
        
        public static IEnumerable<TRecordedCall> ForEach<TRecordedCall>(this IEnumerable<TRecordedCall> recordedCalls, Action<TRecordedCall, int> visitor)
            where TRecordedCall : IRecordedCall
        {
            return recordedCalls.Select((call, i) =>
            {
                visitor.Invoke(call, i);

                return call;
            });
        }
    }
}