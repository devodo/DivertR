using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class InvocationState<T> where T : class
    {
        public T Origin { get; }
        private readonly IEnumerator<Substitution<T>> _substitutionsEnumerator;

        public InvocationState(T origin, IEnumerable<Substitution<T>> substitutions, IInvocation invocation)
        {
            Origin = origin;
            _substitutionsEnumerator = substitutions.Where(x => x.IsMatch(invocation)).GetEnumerator();
        }

        public Substitution<T> Previous()
        {
            return !_substitutionsEnumerator.MoveNext() ? null : _substitutionsEnumerator.Current;
        }
    }
}
