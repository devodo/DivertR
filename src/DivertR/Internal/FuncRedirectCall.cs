using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncRedirectCall<TTarget, TReturn> : IFuncRedirectCall<TTarget, TReturn> where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FuncRedirectCall(CallInfo<TTarget> callInfo, IRelay<TTarget, TReturn> relay)
        {
            CallInfo = callInfo;
            Relay = relay;
        }
        
        public CallInfo<TTarget> CallInfo { get; }
        public CallArguments Args => CallInfo.Arguments;
        public IRelay<TTarget, TReturn> Relay { get; }
    }
    
    internal class FuncRedirectCall<TTarget, TReturn, TArgs> : FuncRedirectCall<TTarget, TReturn>, IFuncRedirectCall<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FuncRedirectCall(CallInfo<TTarget> callInfo, IRelay<TTarget, TReturn> relay, TArgs args) : base(callInfo, relay)
        {
            Args = args;
        }
        
        public new TArgs Args { get; }
    }
}