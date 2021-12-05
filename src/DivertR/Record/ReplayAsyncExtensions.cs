using System;
using System.Threading.Tasks;
using DivertR.Record.Internal;

namespace DivertR.Record
{
    public static class ReplayAsyncExtensions
    {
        public static async Task<IReplayResult> ReplayAsync<TMap>(this ICallStream<Task<TMap>> recordedCalls, Action<TMap> visitor)
        {
            var count = 0;
            
            foreach (var call in recordedCalls)
            {
                visitor.Invoke(await call);
                count++;
            }

            return new ReplayResult(count);
        }

        public static async Task<IReplayResult> ReplayAsync<TMap>(this ICallStream<Task<TMap>> recordedCalls, Action<TMap, int> visitor)
        {
            var count = 0;
            
            foreach (var call in recordedCalls)
            {
                visitor.Invoke(await call, count++);
            }

            return new ReplayResult(count);
        }
        
        public static async Task<IReplayResult> ReplayAsync<TMap>(this ICallStream<Task<TMap>> recordedCalls, Func<TMap, Task> visitor)
        {
            var count = 0;
            
            foreach (var callTask in recordedCalls)
            {
                var call = await callTask.ConfigureAwait(false);
                await visitor.Invoke(call).ConfigureAwait(false);
                count++;
            }

            return new ReplayResult(count);
        }

        public static async Task<IReplayResult> ReplayAsync<TMap>(this ICallStream<Task<TMap>> recordedCalls, Func<TMap, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var callTask in recordedCalls)
            {
                var call = await callTask.ConfigureAwait(false);
                await visitor.Invoke(call, count++).ConfigureAwait(false);
            }

            return new ReplayResult(count);
        }
    }
}