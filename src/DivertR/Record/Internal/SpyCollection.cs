using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class SpyCollection<TMap> : ISpyCollection<TMap>
    {
        private readonly ConcurrentQueue<TMap> _mappedCalls;

        public SpyCollection(ConcurrentQueue<TMap> mappedCalls)
        {
            _mappedCalls = mappedCalls;
        }

        public int Count => _mappedCalls.Count;
        
        public IEnumerator<TMap> GetEnumerator()
        {
            return _mappedCalls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Scan(Action<TMap> visitor)
        {
            return _mappedCalls.Select(call =>
            {
                visitor.Invoke(call);

                return call;
            }).Count();
        }

        public int Scan(Action<TMap, int> visitor)
        {
            return _mappedCalls.Select((call, i) =>
            {
                visitor.Invoke(call, i);

                return call;
            }).Count();
        }
        
        public async Task<int> ScanAsync(Func<TMap, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in _mappedCalls)
            {
                await visitor.Invoke(call).ConfigureAwait(false);
                count++;
            }

            return count;
        }

        public async Task<int> ScanAsync(Func<TMap, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in _mappedCalls)
            {
                await visitor.Invoke(call, count++).ConfigureAwait(false);
            }

            return count;
        }
    }
}