using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ActionRedirectCall<TTarget> : RedirectCall<TTarget>, IActionRedirectCall<TTarget> where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ActionRedirectCall(IRedirectCall<TTarget> call) : base(call.Relay, call.CallInfo)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void CallNext()
        {
            base.CallNext();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void CallNext(CallArguments args)
        {
            base.CallNext(args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void CallRoot()
        {
            base.CallRoot();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void CallRoot(CallArguments args)
        {
            base.CallRoot(args);
        }
    }
    
    internal class ActionRedirectCall<TTarget, TArgs> : ActionRedirectCall<TTarget>, IActionRedirectCall<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ActionRedirectCall(IRedirectCall<TTarget> redirectCall, TArgs args) : base(redirectCall)
        {
            Args = args;
        }

        public new TArgs Args
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}