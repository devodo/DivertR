using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class CallInfo<TTarget> where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CallInfo(TTarget proxy, TTarget? original, MethodInfo method, CallArguments args)
        {
            Proxy = proxy;
            Original = original;
            Method = method;
            Arguments = args;
        }

        public TTarget Proxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public TTarget? Original
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public MethodInfo Method
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public CallArguments Arguments
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}