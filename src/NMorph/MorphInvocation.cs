using System;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class MorphInvocation<T> : IMorphInvocation<T> where T : class
    {
        private readonly InvocationStack<T> _invocationStack;
        private readonly T _replacedTarget;

        public MorphInvocation(InvocationStack<T> invocationStack, T replacedTarget = null)
        {
            _invocationStack = invocationStack;
            _replacedTarget = replacedTarget;
        }

        public T ReplacedTarget => _replacedTarget ?? Invocation.OriginTarget;

        public T OriginalTarget => Invocation.OriginTarget;

        private InvocationContext<T> Invocation
        {
            get
            {
                var invocationContext = _invocationStack.Peek();

                if (invocationContext == null)
                {
                    throw new InvalidOperationException("Calls to this instance must only be made in the context of a morph invocation");
                }

                return invocationContext;
            }
        }
    }
}