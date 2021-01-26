using Castle.DynamicProxy;

namespace Divertr
{
    public interface IInvocationCondition<T> where T : class
    {
        bool IsMatch(IInvocation invocation);
    }
}