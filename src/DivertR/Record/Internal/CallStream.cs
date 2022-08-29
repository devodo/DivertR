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
            Calls = calls ?? throw new ArgumentNullException(nameof(calls));
        }

        public ICallStream<T> Where(Func<T, bool> predicate)
        {
            return new CallStream<T>(Calls.Where(predicate));
        }

        public ICallStream<TMap> Map<TMap>(Func<T, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(mapper.Invoke));
        }

        public IVerifySnapshot<T> Verify()
        {
            var snapshot = Calls.ToList();

            return new VerifySnapshot<T>(snapshot);
        }

        public IVerifySnapshot<T> Verify(Action<T> visitor)
        {
            var snapshot = Calls.ToList();

            foreach (var call in snapshot)
            {
                visitor.Invoke(call);
            }

            return new VerifySnapshot<T>(snapshot);
        }

        public async Task<IVerifySnapshot<T>> Verify(Func<T, Task> visitor)
        {
            var snapshot = Calls.ToList();

            foreach (var call in snapshot)
            {
                await visitor.Invoke(call).ConfigureAwait(false);
            }

            return new VerifySnapshot<T>(snapshot);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Calls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => Calls.Count();
    }
}