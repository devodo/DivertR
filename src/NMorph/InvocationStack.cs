using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NMorph
{
    internal class InvocationStack<T> where T : class
    {
        private readonly AsyncLocal<List<InvocationContext<T>>> _invocationStack = new AsyncLocal<List<InvocationContext<T>>>();
        
        public void Push(InvocationContext<T> invocationContext)
        {
            var invocationStack = _invocationStack.Value?.ToList() ?? new List<InvocationContext<T>>();
            invocationStack.Add(invocationContext);
            _invocationStack.Value = invocationStack;
        }

        public InvocationContext<T> Pop()
        {
            var invocationStack = _invocationStack.Value;

            if (invocationStack == null || invocationStack.Count == 0)
            {
                return null;
            }

            var invocationContext = invocationStack[^1];
            invocationStack.RemoveAt(invocationStack.Count - 1);

            return invocationContext;
        }

        public InvocationContext<T> Peek()
        {
            var invocationStack = _invocationStack.Value;

            return invocationStack?.FirstOrDefault();
        }
    }
}