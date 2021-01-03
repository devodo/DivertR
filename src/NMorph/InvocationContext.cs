using Castle.DynamicProxy;

namespace NMorph
{
    internal class InvocationContext<T> : IInvocationContext<T> where T : class
    {
        public IInvocation Invocation { get; }
        public T OriginTarget { get; }

        public InvocationContext(IInvocation invocation, T originTarget)
        {
            Invocation = invocation;
            OriginTarget = originTarget;
        }
    }
}