using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal abstract class AbstractRedirectCall<TCallInfo, TRelay> : IRedirectCall
        where TCallInfo : CallInfo
        where TRelay : IRelay
    {
        protected readonly TRelay RelayInternal;
        protected readonly TCallInfo CallInfoInternal;

        protected AbstractRedirectCall(TRelay relay, TCallInfo callInfo, IRedirect redirect)
        {
            RelayInternal = relay;
            CallInfoInternal = callInfo;
            Redirect = redirect;
        }
        
        public IRelay Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => RelayInternal;
        }
        
        public CallInfo CallInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CallInfoInternal;
        }
        
        public CallArguments Args
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CallInfo.Arguments;
        }
        
        public IRedirect Redirect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
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
    

    internal class RedirectCall<TTarget> : AbstractRedirectCall<CallInfo<TTarget>, IRelay<TTarget>>, IRedirectCall<TTarget>
        where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RedirectCall(IRelay<TTarget> relay, CallInfo<TTarget> callInfo, IRedirect redirect) : base(relay, callInfo, redirect)
        {
        }
        
        public new IRelay<TTarget> Relay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => RelayInternal;
        }

        public new CallInfo<TTarget> CallInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CallInfoInternal;
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