using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class CallLog<TMap> : ICallLog<TMap>
    {
        protected readonly IReadOnlyCollection<TMap> Calls;

        public CallLog(IReadOnlyCollection<TMap> calls)
        {
            Calls = calls;
        }

        public IEnumerator<TMap> GetEnumerator() => Calls.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => Calls.Count;
        
        public int Replay(Action<TMap> visitor)
        {
            return this.Select(call =>
            {
                visitor.Invoke(call);

                return call;
            }).Count();
        }

        public int Replay(Action<TMap, int> visitor)
        {
            return this.Select((call, i) =>
            {
                visitor.Invoke(call, i);

                return call;
            }).Count();
        }
        
        public async Task<int> Replay(Func<TMap, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in this)
            {
                await visitor.Invoke(call).ConfigureAwait(false);
                count++;
            }

            return count;
        }

        public async Task<int> Replay(Func<TMap, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in this)
            {
                await visitor.Invoke(call, count++).ConfigureAwait(false);
            }

            return count;
        }
    }
}