using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NMorph
{
    internal class InvocationStack<T> where T : class
    {
        private readonly AsyncLocal<List<SubstitutionState<T>>> _invocationStack = new AsyncLocal<List<SubstitutionState<T>>>();
        
        public void Push(SubstitutionState<T> substitutionState)
        {
            var invocationStack = _invocationStack.Value?.ToList() ?? new List<SubstitutionState<T>>();
            invocationStack.Add(substitutionState);
            _invocationStack.Value = invocationStack;
        }

        public SubstitutionState<T> Pop()
        {
            var invocationStack = _invocationStack.Value;

            if (invocationStack == null || invocationStack.Count == 0)
            {
                return null;
            }

            var invocationState = invocationStack[^1];
            invocationStack.RemoveAt(invocationStack.Count - 1);

            return invocationState;
        }

        public SubstitutionState<T> Peek()
        {
            var invocationStack = _invocationStack.Value;

            return invocationStack?[^1];
        }
    }
}