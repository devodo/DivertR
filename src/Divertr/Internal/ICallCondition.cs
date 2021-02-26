using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal interface ICallCondition
    {
        bool IsMatch(IInvocation invocation);
    }
}