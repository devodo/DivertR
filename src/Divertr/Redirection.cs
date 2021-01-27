using Castle.DynamicProxy;

namespace Divertr
{
    internal class Redirection<T> where T : class
    {
        private readonly ICallCondition<T> _callCondition;
        public T Redirect { get; }

        public Redirection(T redirect, ICallCondition<T> callCondition = null)
        {
            _callCondition = callCondition;
            Redirect = redirect;
        }

        public bool IsMatch(IInvocation invocation)
        {
            return _callCondition?.IsMatch(invocation) ?? true;
        }
    }
}
