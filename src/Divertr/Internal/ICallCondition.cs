using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal interface ICallCondition
    {
        bool IsMatch(IInvocation invocation);
    }
}