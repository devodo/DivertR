using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Divertr.Internal
{
    internal class CallContext<T> : ICallContext<T> where T : class
    {
        private readonly AsyncLocal<List<RedirectContext<T>>> _callStack = new AsyncLocal<List<RedirectContext<T>>>();
        public T Next { get; }
        
        public T Root { get; }

        public CallContext()
        {
            Next =  ProxyFactory.Instance.CreateRedirectTargetProxy(this);
            Root = ProxyFactory.Instance.CreateRootTargetProxy(this);
        }
        
        public void Push(RedirectContext<T> redirectContext)
        {
            var callStack = _callStack.Value?.ToList() ?? new List<RedirectContext<T>>();
            callStack.Add(redirectContext);
            _callStack.Value = callStack;
        }

        public RedirectContext<T>? Pop()
        {
            var callStack = _callStack.Value;

            if (callStack == null || callStack.Count == 0)
            {
                return null;
            }

            var invocationState = callStack[callStack.Count - 1];
            callStack.RemoveAt(callStack.Count - 1);

            return invocationState;
        }

        public RedirectContext<T>? Peek()
        {
            var callStack = _callStack.Value;

            return callStack?[callStack.Count - 1];
        }
    }
}