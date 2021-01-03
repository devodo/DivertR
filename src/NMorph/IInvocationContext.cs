using Castle.DynamicProxy;

namespace NMorph
{
    public interface IInvocationContext<T> where T : class
    {
        IInvocation Invocation { get; }
        T OriginTarget { get; }
    }
}