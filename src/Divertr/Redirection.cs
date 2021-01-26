using Castle.DynamicProxy;

namespace Divertr
{
    internal class Redirection<T> where T : class
    {
        private readonly IInvocationCondition<T> _invocationCondition;
        public T RedirectTarget { get; }

        public Redirection(T redirectTarget, IInvocationCondition<T> invocationCondition = null)
        {
            _invocationCondition = invocationCondition;
            RedirectTarget = redirectTarget;
        }

        public bool IsMatch(IInvocation invocation)
        {
            return _invocationCondition?.IsMatch(invocation) ?? true;
        }
    }
}
