using System;
using System.Collections;

namespace DivertR.Internal
{
    internal class FuncRedirectCall<TTarget, TReturn, TArgs> : IFuncRedirectCall<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public FuncRedirectCall(CallInfo<TTarget> callInfo, TArgs args, IRelay<TTarget, TReturn> relay)
        {
            CallInfo = callInfo;
            Args = args;
            Relay = relay;
        }
        
        public CallInfo<TTarget> CallInfo { get; }
        public TArgs Args { get; }
        public IRelay<TTarget, TReturn> Relay { get; }
    }
}