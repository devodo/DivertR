using Castle.DynamicProxy;

namespace NMorph
{
    public interface IInvocationCondition<T> where T : class
    {
        bool IsMatch(IInvocation invocation);
    }
}