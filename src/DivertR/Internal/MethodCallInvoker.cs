using System.Reflection;

namespace DivertR.Internal
{
    internal class MethodCallInvoker : ICallInvoker
    {
        public object? Invoke<TTarget>(TTarget target, MethodInfo method, CallArguments arguments)
        {
            return method.Invoke(target, arguments.InternalArgs);
        }
    }
}