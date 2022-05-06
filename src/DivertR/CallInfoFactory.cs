using System.Reflection;
using System.Runtime.CompilerServices;
using DivertR.Internal;

namespace DivertR
{
    public static class CallInfoFactory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICallInfo<TTarget> Create<TTarget>(TTarget proxy, TTarget? root, MethodInfo method, CallArguments args) where TTarget : class
        {
            return new CallInfo<TTarget>(proxy, root, method, args);
        }
    }
}