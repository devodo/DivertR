using System.Collections.Concurrent;

namespace DivertR.Record.Internal
{
    internal class SpyCollection<TMap> : SpyEnumerable<TMap>, ISpyCollection<TMap>
    {
        private readonly ConcurrentQueue<TMap> _mappedCalls;

        public SpyCollection(ConcurrentQueue<TMap> mappedCalls) : base(mappedCalls)
        {
            _mappedCalls = mappedCalls;
        }

        public int Count => _mappedCalls.Count;
    }
}