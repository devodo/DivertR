using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NMorph
{
    internal class InvocationStack<T> where T : class
    {
        private readonly AsyncLocal<List<InvocationState<T>>> _invocationStack = new AsyncLocal<List<InvocationState<T>>>();
        
        public void Push(InvocationState<T> invocationContext)
        {
            var invocationStack = _invocationStack.Value?.ToList() ?? new List<InvocationState<T>>();
            invocationStack.Add(invocationContext);
            _invocationStack.Value = invocationStack;
        }

        public InvocationState<T> Pop()
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

        public InvocationState<T> Peek()
        {
            var invocationStack = _invocationStack.Value;

            return invocationStack?[^1];
        }
    }
}