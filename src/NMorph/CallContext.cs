using System;

namespace NMorph
{
    internal class CallContext<T> : ICallContext<T> where T : class
    {
        internal InvocationStack<T> InvocationStack { get; }
        private readonly Lazy<T> _substituteProxy;

        public CallContext(InvocationStack<T> invocationStack)
        {
            InvocationStack = invocationStack;
            _substituteProxy = new Lazy<T>(() => ProxyFactory.Instance.CreateSubstitutionProxy(invocationStack));
        }

        public T Previous => _substituteProxy.Value;

        public T Origin => SubstitutionState.Origin;

        private SubstitutionState<T> SubstitutionState
        {
            get
            {
                var substitutionState = InvocationStack.Peek();

                if (substitutionState == null)
                {
                    throw new InvalidOperationException("Calls to this instance must only be made in the context of a morph invocation");
                }

                return substitutionState;
            }
        }
    }
}