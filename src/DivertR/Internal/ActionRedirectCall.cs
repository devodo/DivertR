using System;
using System.Collections;

namespace DivertR.Internal
{
    internal class ActionRedirectCall<TTarget> : IActionRedirectCall<TTarget> where TTarget : class
    {
        public ActionRedirectCall(CallInfo<TTarget> callInfo, IRelay<TTarget> relay)
        {
            CallInfo = callInfo;
            Relay = relay;
        }
        
        public CallInfo<TTarget> CallInfo { get; }
        
        public CallArguments Args => CallInfo.Arguments;
        public IRelay<TTarget> Relay { get; }
    }
    
    internal class ActionRedirectCall<TTarget, TArgs> : IActionRedirectCall<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public ActionRedirectCall(CallInfo<TTarget> callInfo, TArgs args, IRelay<TTarget> relay)
        {
            CallInfo = callInfo;
            Args = args;
            Relay = relay;
        }
        
        public CallInfo<TTarget> CallInfo { get; }
        public TArgs Args { get; }
        public IRelay<TTarget> Relay { get; }
    }
}