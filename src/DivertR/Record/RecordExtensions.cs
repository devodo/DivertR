using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public static class RecordExtensions
    {
        public static int Scan<TRecordedCall>(this IEnumerable<TRecordedCall> recordedCalls, Action<TRecordedCall> visitor)
            where TRecordedCall : IRecordedCall
        {
            return recordedCalls.Select(call =>
            {
                visitor.Invoke(call);

                return call;
            }).Count();
        }
        
        public static int Scan<TRecordedCall>(this IEnumerable<TRecordedCall> recordedCalls, Action<TRecordedCall, int> visitor)
            where TRecordedCall : IRecordedCall
        {
            return recordedCalls.Select((call, i) =>
            {
                visitor.Invoke(call, i);

                return call;
            }).Count();
        }
        
        public static async Task<int> ScanAsync<TRecordedCall>(this IEnumerable<TRecordedCall> recordedCalls, Func<TRecordedCall, Task> visitor)
            where TRecordedCall : IRecordedCall
        {
            var count = 0;
            
            foreach (var call in recordedCalls)
            {
                await visitor.Invoke(call).ConfigureAwait(false);
                count++;
            }

            return count;
        }

        public static async Task<int> ScanAsync<TRecordedCall>(this IEnumerable<TRecordedCall> recordedCalls, Func<TRecordedCall, int, Task> visitor)
            where TRecordedCall : IRecordedCall
        {
            var count = 0;
            
            foreach (var call in recordedCalls)
            {
                await visitor.Invoke(call, count++).ConfigureAwait(false);
            }

            return count;
        }
        
        public static async Task<int> ScanAsync<TMap>(this ISpyCollection<Task<TMap>> recordedCalls, Action<TMap> visitor)
        {
            var count = 0;
            
            foreach (var call in recordedCalls)
            {
                visitor.Invoke(await call);
                count++;
            }

            return count;
        }

        public static async Task<int> ScanAsync<TMap>(this ISpyCollection<Task<TMap>> recordedCalls, Action<TMap, int> visitor)
        {
            var count = 0;
            
            foreach (var call in recordedCalls)
            {
                visitor.Invoke(await call, count++);
            }

            return count;
        }
        
        public static async Task<int> ScanAsync<TMap>(this ISpyCollection<Task<TMap>> recordedCalls, Func<TMap, Task> visitor)
        {
            var count = 0;
            
            foreach (var callTask in recordedCalls)
            {
                var call = await callTask.ConfigureAwait(false);
                await visitor.Invoke(call).ConfigureAwait(false);
                count++;
            }

            return count;
        }

        public static async Task<int> ScanAsync<TMap>(this ISpyCollection<Task<TMap>> recordedCalls, Func<TMap, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var callTask in recordedCalls)
            {
                var call = await callTask.ConfigureAwait(false);
                await visitor.Invoke(call, count++).ConfigureAwait(false);
            }

            return count;
        }
    }
}