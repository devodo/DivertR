using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class SubstitutionState<T> where T : class
    {
        private readonly AsyncLocal<List<int>> _indexStack = new AsyncLocal<List<int>>();
        public T Origin { get; }
        private readonly List<Substitution<T>> _substitutions;

        public SubstitutionState(T origin, List<Substitution<T>> substitutions)
        {
            _substitutions = substitutions;
            Origin = origin;
        }

        public T MoveNext(IInvocation invocation)
        {
            T substitute = null;
            
            var indexStack = _indexStack.Value;
            var i = indexStack?.LastOrDefault() ?? 0;

            if (i >= _substitutions.Count)
            {
                return Origin;
            }
            
            for (; i < _substitutions.Count; i++)
            {
                if (_substitutions[i].IsMatch(invocation))
                {
                    substitute = _substitutions[i].Substitute;
                    break;
                }
            }

            _indexStack.Value = (indexStack ?? new List<int>()).Append(i + 1).ToList();

            return substitute;
        }

        public void MoveBack()
        {
            var indexStack = _indexStack.Value;

            if (indexStack == null || indexStack.Count == 0)
            {
                return;
            }
            
            indexStack.RemoveAt(indexStack.Count - 1);
        }
    }
}
