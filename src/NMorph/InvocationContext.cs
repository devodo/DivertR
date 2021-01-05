using System;

namespace NMorph
{
    internal class InvocationContext<T> : IInvocationContext<T> where T : class
    {
        private readonly InvocationStack<T> _invocationStack;
        private readonly Lazy<Substitution<T>> _previousSubstitute;

        public InvocationContext(InvocationStack<T> invocationStack)
        {
            _invocationStack = invocationStack;
            _previousSubstitute = new Lazy<Substitution<T>>(() => InvocationState.Previous());
        }

        public T Previous => _previousSubstitute.Value?.Substitute ?? InvocationState.Origin;

        public T Origin => InvocationState.Origin;

        private InvocationState<T> InvocationState
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