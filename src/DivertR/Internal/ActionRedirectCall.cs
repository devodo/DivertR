using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ActionRedirectCall<TTarget> : IActionRedirectCall<TTarget> where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ActionRedirectCall(CallInfo<TTarget> callInfo, IRelay<TTarget> relay)
        {
            CallInfo = callInfo;
            Relay = relay;
        }

        public CallInfo<TTarget> CallInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        public CallArguments Args
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CallInfo.Arguments;
        }

        public IRelay<TTarget> Relay
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
    
    internal class ActionRedirectCall<TTarget, TArgs> : ActionRedirectCall<TTarget>, IActionRedirectCall<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ActionRedirectCall(CallInfo<TTarget> callInfo, IRelay<TTarget> relay, TArgs args) : base(callInfo, relay)
        {
            Args = args;
        }

        CallArguments IActionRedirectCall<TTarget>.Args
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => base.Args;
        }

        public new TArgs Args
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}