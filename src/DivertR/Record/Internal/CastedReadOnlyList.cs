using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record.Internal
{
    internal class CastedReadOnlyList<TDerived> : IReadOnlyList<TDerived>
    {
        private readonly IReadOnlyList<object> _baseArray;

        public CastedReadOnlyList(IReadOnlyList<object> baseArray)
        {
            _baseArray = baseArray;
        }

        public IEnumerator<TDerived> GetEnumerator()
        {
            return new CastedEnumerator<TDerived>(_baseArray.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _baseArray.GetEnumerator();
        }

        public int Count => _baseArray.Count;

        public TDerived this[int index] => (TDerived) _baseArray[index]!;
    }

    internal class CastedEnumerator<TDerived> : IEnumerator<TDerived>
    {
        private readonly IEnumerator<object> _innerEnumerator;

        public CastedEnumerator(IEnumerator<object> innerEnumerator)
        {
            _innerEnumerator = innerEnumerator;
        }
        
        public bool MoveNext()
        {
            return _innerEnumerator.MoveNext();
        }

        public void Reset()
        {
            _innerEnumerator.Reset();
        }

        public TDerived Current => (TDerived) _innerEnumerator.Current!;

        object IEnumerator.Current => _innerEnumerator.Current!;

        public void Dispose()
        {
            _innerEnumerator.Dispose();
        }
    }
}
