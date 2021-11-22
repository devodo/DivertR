using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DivertR.Record.Internal
{
    internal class SpyEnumerable<TMap> : ISpyEnumerable<TMap>
    {
        private readonly IEnumerable<TMap> _mappedCalls;

        public SpyEnumerable(IEnumerable<TMap> mappedCalls)
        {
            _mappedCalls = mappedCalls;
        }
        
        public IEnumerator<TMap> GetEnumerator()
        {
            return _mappedCalls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ISpyEnumerable<TMap> ForEach(Action<TMap> visitor)
        {
            return new SpyEnumerable<TMap>(_mappedCalls.Select(mappedCall =>
            {
                visitor.Invoke(mappedCall);

                return mappedCall;
            }));
        }

        public ISpyEnumerable<TMap> ForEach(Action<TMap, int> visitor)
        {
            return new SpyEnumerable<TMap>(_mappedCalls.Select((mappedCall, i) =>
            {
                visitor.Invoke(mappedCall, i);

                return mappedCall;
            }));
        }
    }
}