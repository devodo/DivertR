namespace NMorph
{
    internal class Alteration {}

    internal class Alteration<T> : Alteration where T : class
    {
        public T Substitute { get; }
        public InvocationStack<T> InvocationStack { get; }

        public Alteration(T substitute, InvocationStack<T> invocationStack)
        {
            Substitute = substitute;
            InvocationStack = invocationStack;
        }
    }
}