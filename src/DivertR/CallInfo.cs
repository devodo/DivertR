using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public abstract class CallInfo
    {
        protected CallInfo(object proxy, object? root, MethodInfo method, CallArguments args)
        {
            Proxy = proxy;
            Root = root;
            Method = method;
            Arguments = args;
        }
        
        public object Proxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public object? Root
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

        public CallInfo Create(CallArguments args)
        {
            return Create(Method, args);
        }

        public abstract CallInfo Create(MethodInfo method, CallArguments args);
    }
    
    public class CallInfo<TTarget> : CallInfo where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CallInfo(TTarget proxy, TTarget? root, MethodInfo method, CallArguments args)
            : base(proxy, root, method, args)
        {
        }

        public new TTarget Proxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (base.Proxy as TTarget)!;
        }

        public new TTarget? Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => base.Root as TTarget;
        }

        public override CallInfo Create(MethodInfo method, CallArguments args)
        {
            return new CallInfo<TTarget>(Proxy, Root, method, args);
        }
    }
}