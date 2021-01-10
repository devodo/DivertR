using Castle.DynamicProxy;

namespace NMorph
{
    internal class Substitution<T> where T : class
    {
        private readonly IInvocationCondition<T> _invocationCondition;
        public T Substitute { get; }

        public Substitution(T substitute, IInvocationCondition<T> invocationCondition)
        {
            _invocationCondition = invocationCondition;
            Substitute = substitute;
        }

        public bool IsMatch(IInvocation invocation)
        {
            return _invocationCondition?.IsMatch(invocation) ?? true;
        }
    }
}
