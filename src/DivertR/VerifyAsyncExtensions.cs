using System;
using System.Threading.Tasks;
using DivertR.Record;

namespace DivertR
{
    public static class VerifyAsyncExtensions
    {
        public static async Task<IVerifySnapshot<Task<TMap>>> VerifyAsync<TMap>(this ICallStream<Task<TMap>> callStream, Action<TMap> visitor)
        {
            var verifySnapshot = callStream.Verify();
            
            foreach (var callTask in verifySnapshot)
            {
                var call = await callTask.ConfigureAwait(false);
                visitor.Invoke(call);
            }

            return verifySnapshot;
        }

        public static async Task<IVerifySnapshot<Task<TMap>>> VerifyAsync<TMap>(this ICallStream<Task<TMap>> callStream, Func<TMap, Task> visitor)
        {
            var verifySnapshot = callStream.Verify();
            
            foreach (var callTask in verifySnapshot)
            {
                var call = await callTask.ConfigureAwait(false);
                await visitor.Invoke(call).ConfigureAwait(false);
            }

            return verifySnapshot;
        }
    }
}