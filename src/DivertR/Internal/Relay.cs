using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal abstract class AbstractRelay<TCallInfo, TRedirect, TRedirectCall> : IRelay
        where TCallInfo : CallInfo
        where TRedirect : IRedirect<TCallInfo, TRedirectCall>
        where TRedirectCall : IRedirectCall
    {
        private readonly AsyncLocal<ImmutableStack<RelayIndex<TCallInfo, TRedirect>>> _relayIndexStack = new AsyncLocal<ImmutableStack<RelayIndex<TCallInfo, TRedirect>>>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallRoot()
        {
            var relayIndex = GetRelayIndexStack().Peek();
            ValidateStrict(relayIndex);

            return CallRoot(relayIndex.CallInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallRoot(MethodInfo method, CallArguments args)
        {
            var relayIndex = GetRelayIndexStack().Peek();
            ValidateStrict(relayIndex);
            var callInfo = relayIndex.CallInfo.Create(method, args);
            
            return CallRoot(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallRoot(CallArguments args)
        {
            var relayIndex = GetRelayIndexStack().Peek();
            ValidateStrict(relayIndex);
            var callInfo = relayIndex.CallInfo.Create(args);
            
            return CallRoot(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRedirectCall GetCurrentCall()
        {
            var relayIndex = GetRelayIndexStack().Peek();
            
            return CreateRedirectCall(relayIndex.CallInfo, relayIndex.Redirect);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext()
        {
            var relayIndexStack = GetRelayIndexStack();
            var relayIndex = relayIndexStack.Peek();
            var relayNext = relayIndex.MoveNext(relayIndex.CallInfo);

            if (relayNext == null)
            {
                ValidateStrict(relayIndex);

                return CallRoot(relayIndex.CallInfo);
            }
            
            relayIndexStack = relayIndexStack.Push(relayNext);
            _relayIndexStack.Value = relayIndexStack;

            try
            {
                return relayNext.Redirect.Handle(CreateRedirectCall(relayIndex.CallInfo, relayNext.Redirect));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(MethodInfo method, CallArguments args)
        {
            var relayIndexStack = GetRelayIndexStack();
            var relayIndex = relayIndexStack.Peek();
            var callInfo = (TCallInfo) relayIndex.CallInfo.Create(method, args);
            var relayNext = relayIndex.MoveNext(callInfo);

            if (relayNext == null)
            {
                ValidateStrict(relayIndex);

                return CallRoot(callInfo);
            }
            
            relayIndexStack = relayIndexStack.Push(relayNext);
            _relayIndexStack.Value = relayIndexStack;

            try
            {
                return relayNext.Redirect.Handle(CreateRedirectCall(callInfo, relayNext.Redirect));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(CallArguments args)
        {
            var relayIndexStack = GetRelayIndexStack();
            var relayIndex = relayIndexStack.Peek();
            var callInfo = (TCallInfo) relayIndex.CallInfo.Create(args);
            var relayNext = relayIndex.MoveNext(callInfo);

            if (relayNext == null)
            {
                ValidateStrict(relayIndex);

                return CallRoot(callInfo);
            }
            
            relayIndexStack = relayIndexStack.Push(relayNext);
            _relayIndexStack.Value = relayIndexStack;

            try
            {
                return relayNext.Redirect.Handle(CreateRedirectCall(callInfo, relayNext.Redirect));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal object? CallBegin(RedirectPlan<TRedirect> redirectPlan, TCallInfo callInfo)
        {
            var relayIndex = RelayIndex<TCallInfo, TRedirect>.Create(redirectPlan, callInfo);

            if (relayIndex == null)
            {
                if (redirectPlan.IsStrictMode)
                {
                    throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any redirects");
                }
                
                return CallRoot(callInfo);
            }
            
            var relayIndexStack = _relayIndexStack.Value ?? ImmutableStack<RelayIndex<TCallInfo, TRedirect>>.Empty;
            relayIndexStack = relayIndexStack.Push(relayIndex);
            _relayIndexStack.Value = relayIndexStack;
            
            try
            {
                return relayIndex.Redirect.Handle(CreateRedirectCall(callInfo, relayIndex.Redirect));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableStack<RelayIndex<TCallInfo, TRedirect>> GetRelayIndexStack()
        {
            var relayIndexStack = _relayIndexStack.Value;

            if (relayIndexStack == null || relayIndexStack.IsEmpty)
            {
                // The AsyncLocal value has not been initialised in the current context, therefore the caller in not within a call on this Relay.
                throw new DiverterException("Access to this member is only valid within the context of a redirect call");
            }

            return relayIndexStack;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateStrict(RelayIndex<TCallInfo, TRedirect> relayIndex)
        {
            if (!relayIndex.StrictSatisfied)
            {
                throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any redirects");
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object? CallRoot(CallInfo callInfo)
        {
            if (callInfo.Root == null)
            {
                throw new DiverterNullRootException("Root instance is null");
            }

            return callInfo.Invoke(callInfo.Root);
        }

        protected abstract object? CallRedirect(TCallInfo callInfo, TRedirect redirect);
        protected abstract TRedirectCall CreateRedirectCall(TCallInfo callInfo, TRedirect redirect);
    }
    
    internal class Relay<TTarget> : AbstractRelay<CallInfo<TTarget>, Redirect<TTarget>, IRedirectCall<TTarget>>, IRelay<TTarget>
        where TTarget : class
    {
        private readonly Lazy<TTarget> _next;
        private readonly Lazy<TTarget> _root;
        
        public TTarget Next
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _next.Value;
        }
        
        public TTarget Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _root.Value;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new IRedirectCall<TTarget> GetCurrentCall()
        {
            return (IRedirectCall<TTarget>) base.GetCurrentCall();
        }

        protected override object? CallRedirect(CallInfo<TTarget> callInfo, Redirect<TTarget> redirect)
        {
            var redirectCall = new RedirectCall<TTarget>(this, callInfo, redirect);

            return redirect.Handle(redirectCall);
        }

        protected override IRedirectCall<TTarget> CreateRedirectCall(CallInfo<TTarget> callInfo, Redirect<TTarget> redirect)
        {
            return new RedirectCall<TTarget>(this, callInfo, redirect);
        }

        public Relay(IProxyFactory proxyFactory)
        {
            _next = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new NextProxyCall<TTarget>(this)));
            _root = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new RootProxyCall<TTarget>(this)));
        }
    }
}