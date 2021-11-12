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
    
    internal class ActionRedirectCall<TTarget, TArgs> : ActionRedirectCall<TTarget>, IActionRedirectCall<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public ActionRedirectCall(CallInfo<TTarget> callInfo, IRelay<TTarget> relay, TArgs args) : base(callInfo, relay)
        {
            Args = args;
        }

        CallArguments IActionRedirectCall<TTarget>.Args => base.Args;
        
        public new TArgs Args { get; }
    }
}