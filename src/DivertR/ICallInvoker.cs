using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface ICallInvoker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? Invoke<TTarget>(TTarget target, MethodInfo method, CallArguments arguments);
    }
}