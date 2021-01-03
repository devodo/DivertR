namespace NMorph
{
    internal class MorphSource<T> : IMorphSource<T> where T : class
    {
        private readonly InvocationStack<T> _invocationStack;
        private readonly T _replacedTarget;

        public MorphSource(InvocationStack<T> invocationStack, T replacedTarget = null)
        {
            _invocationStack = invocationStack;
            _replacedTarget = replacedTarget;
        }

        public T ReplacedTarget => _replacedTarget ?? Invocation?.OriginTarget;

        public IInvocationContext<T> Invocation => _invocationStack.Peek();
    }
}