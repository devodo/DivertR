using Castle.DynamicProxy;

namespace Divertr
{
    public interface ICallCondition<T> where T : class
    {
        bool IsMatch(IInvocation invocation);
    }
}