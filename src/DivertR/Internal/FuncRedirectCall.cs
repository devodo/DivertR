using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncRedirectCall<TTarget, TReturn> : RedirectCall<TTarget>, IFuncRedirectCall<TTarget, TReturn> where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FuncRedirectCall(IRedirectCall<TTarget> call) : base(call.Relay, call.CallInfo)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new TReturn CallNext()
        {
            return (TReturn) base.CallNext()!;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new TReturn CallNext(CallArguments args)
        {
            return (TReturn) base.CallNext(args)!;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new TReturn CallRoot()
        {
            return (TReturn) base.CallRoot()!;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new TReturn CallRoot(CallArguments args)
        {
            return (TReturn) base.CallRoot(args)!;
        }
    }

    internal class FuncRedirectCall<TTarget, TReturn, TArgs> : FuncRedirectCall<TTarget, TReturn>, IFuncRedirectCall<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FuncRedirectCall(IRedirectCall<TTarget> call, TArgs args) : base(call)
        {
            Args = args;
        }

        public new TArgs Args
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
    
    internal class FuncRedirectCall<TReturn> : RedirectCall, IFuncRedirectCall<TReturn>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FuncRedirectCall(IRedirectCall call) : base(call.Relay, call.CallInfo)
        {
        }
        
        [return: MaybeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new TReturn CallNext()
        {
            return (TReturn) base.CallNext()!;
        }
        
        [return: MaybeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new TReturn CallNext(CallArguments args)
        {
            return (TReturn) base.CallNext(args)!;
        }
        
        [return: MaybeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new TReturn CallRoot()
        {
            return (TReturn) base.CallRoot()!;
        }
        
        [return: MaybeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new TReturn CallRoot(CallArguments args)
        {
            return (TReturn) base.CallRoot(args)!;
        }
    }
}