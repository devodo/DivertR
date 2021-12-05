using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class CallStream<T> : ICallStream<T>
    {
        protected readonly IEnumerable<T> Calls;

        public CallStream(IEnumerable<T> calls)
        {
            Calls = calls;
        }

        public ICallStream<TMap> Map<TMap>(Func<T, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(mapper.Invoke));
        }

        public IReplayResult Replay(Action<T> visitor)
        {
            return new ReplayResult(this.Select(call =>
            {
                visitor.Invoke(call);

                return call;
            }).Count());
        }

        public IReplayResult Replay(Action<T, int> visitor)
        {
            return new ReplayResult(this.Select((call, i) =>
            {
                visitor.Invoke(call, i);

                return call;
            }).Count());
        }
        
        public async Task<IReplayResult> Replay(Func<T, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in this)
            {
                await visitor.Invoke(call).ConfigureAwait(false);
                count++;
            }

            return new ReplayResult(count);
        }

        public async Task<IReplayResult> Replay(Func<T, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in this)
            {
                await visitor.Invoke(call, count++).ConfigureAwait(false);
            }

            return new ReplayResult(count);
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return Calls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}