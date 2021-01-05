using Castle.DynamicProxy;

namespace NMorph
{
    internal class Substitution<T> where T : class
    {
        public T Substitute { get; }

        public Substitution(T substitute)
        {
            Substitute = substitute;
        }

        public bool IsMatch(IInvocation invocation)
        {
            return true;
        }
    }
}
