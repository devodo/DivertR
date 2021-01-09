using System;

namespace NMorph
{
    internal class InvocationContext<T> : IInvocationContext<T> where T : class
    {
        private readonly InvocationStack<T> _invocationStack;
        private readonly Lazy<T> _substituteProxy;

        public InvocationContext(InvocationStack<T> invocationStack)
        {
            _invocationStack = invocationStack;
            _substituteProxy = new Lazy<T>(() => ProxyFactory.Instance.CreateSubstitutionProxy(invocationStack));
        }

        public T Previous => _substituteProxy.Value;

        public T Origin => SubstitutionState.Origin;

        private SubstitutionState<T> SubstitutionState
        {
            get
            {
                var invocationState = _invocationStack.Peek();

                if (invocationState == null)
                {
                    throw new InvalidOperationException("Calls to this instance must only be made in the context of a morph invocation");
                }

                return invocationState;
            }
        }
    }
}