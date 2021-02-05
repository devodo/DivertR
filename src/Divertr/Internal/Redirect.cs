using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class Redirect<T> where T : class
    {
        private readonly ICallCondition<T>? _callCondition;
        public T Target { get; }

        public Redirect(T target, ICallCondition<T>? callCondition = null)
        {
            _callCondition = callCondition;
            Target = target;
        }

        public bool IsMatch(IInvocation invocation)
        {
            return _callCondition?.IsMatch(invocation) ?? true;
        }
    }
}
