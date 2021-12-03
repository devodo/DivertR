using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DivertR.Record.Internal
{
    internal class MappedCollection<TInput, TMap> : IReadOnlyCollection<TMap>
    {
        private readonly IReadOnlyCollection<TInput> _inputs;
        private readonly Func<TInput, TMap> _mapper;

        public MappedCollection(IReadOnlyCollection<TInput> inputs, Func<TInput, TMap> mapper)
        {
            _inputs = inputs;
            _mapper = mapper;
        }

        public IEnumerator<TMap> GetEnumerator()
        {
            return _inputs.Select(input => _mapper.Invoke(input)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _inputs.Count;
    }
}