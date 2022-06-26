using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectCall : IRedirectCall
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected RedirectCall(IRelay relay, ICallInfo callInfo)
        {
            Relay = relay;
            CallInfo = callInfo;
        }
        
        public IRelay Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        public ICallInfo CallInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        public CallArguments Args
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CallInfo.Arguments;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext()
        {
            return Relay.CallNext();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(MethodInfo method, CallArguments args)
        {
            return Relay.CallNext(method, args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(CallArguments args)
        {
            return Relay.CallNext(args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallRoot()
        {
            return Relay.CallRoot();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallRoot(MethodInfo method, CallArguments args)
        {
            return Relay.CallRoot(method, args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallRoot(CallArguments args)
        {
            return Relay.CallRoot(args);
        }
    }
    
    internal class RedirectCall<TTarget> : RedirectCall, IRedirectCall<TTarget>
        where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RedirectCall(IRelay<TTarget> relay, ICallInfo<TTarget> callInfo) : base(relay, callInfo)
        {
            Relay = relay;
            CallInfo = callInfo;
        }
        
        public new IRelay<TTarget> Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        public new ICallInfo<TTarget> CallInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public TTarget Next
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Relay.Next;
        }

        public TTarget Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Relay.Root;
        }
    }
}