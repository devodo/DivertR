using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal class Relay<TTarget> : IRelay<TTarget> where TTarget : class
    {
        private readonly AsyncLocal<ImmutableStack<RelayIndex<TTarget>>> _relayIndexStack = new AsyncLocal<ImmutableStack<RelayIndex<TTarget>>>();
        
        private readonly Lazy<TTarget> _next;
        private readonly Lazy<TTarget> _root;
        
        public TTarget Next
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _next.Value;
        }
        
        object IRelay.Next
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _next.Value;
        }

        public TTarget Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _root.Value;
        }
        
        object IRelay.Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _root.Value;
        }
        
        public Relay(IProxyFactory proxyFactory)
        {
            _next = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new NextProxyCall<TTarget>(this)));
            _root = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new RootProxyCall<TTarget>(this)));
        }

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
            var callInfo = new CallInfo<TTarget>(relayIndex.CallInfo.Proxy, relayIndex.CallInfo.Root, method, args);
            
            return CallRoot(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallRoot(CallArguments args)
        {
            var relayIndex = GetRelayIndexStack().Peek();
            ValidateStrict(relayIndex);
            var callInfo = new CallInfo<TTarget>(relayIndex.CallInfo.Proxy, relayIndex.CallInfo.Root, relayIndex.CallInfo.Method, args);
            
            return CallRoot(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRedirectCall<TTarget> GetCurrentCall()
        {
            var relayIndex = GetRelayIndexStack().Peek();
            return new RedirectCall<TTarget>(this, relayIndex.CallInfo, relayIndex.Redirect);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IRedirectCall IRelay.GetCurrentCall()
        {
            return GetCurrentCall();
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
                return relayNext.Redirect.CallHandler.Call(new RedirectCall<TTarget>(this, relayIndex.CallInfo, relayNext.Redirect));
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
            var callInfo = new CallInfo<TTarget>(relayIndex.CallInfo.Proxy, relayIndex.CallInfo.Root, method, args);
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
                return relayNext.Redirect.CallHandler.Call(new RedirectCall<TTarget>(this, callInfo, relayNext.Redirect));
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
            var callInfo = new CallInfo<TTarget>(relayIndex.CallInfo.Proxy, relayIndex.CallInfo.Root, relayIndex.CallInfo.Method, args);
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
                return relayNext.Redirect.CallHandler.Call(new RedirectCall<TTarget>(this, callInfo, relayNext.Redirect));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal object? CallBegin(RedirectPlan redirectPlan, CallInfo<TTarget> callInfo)
        {
            var relayIndex = RelayIndex<TTarget>.Create(redirectPlan, callInfo);

            if (relayIndex == null)
            {
                if (redirectPlan.IsStrictMode)
                {
                    throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any redirects");
                }
                
                return CallRoot(callInfo);
            }
            
            var relayIndexStack = _relayIndexStack.Value ?? ImmutableStack<RelayIndex<TTarget>>.Empty;
            relayIndexStack = relayIndexStack.Push(relayIndex);
            _relayIndexStack.Value = relayIndexStack;
            
            try
            {
                return relayIndex.Redirect.CallHandler.Call(new RedirectCall<TTarget>(this, callInfo, relayIndex.Redirect));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableStack<RelayIndex<TTarget>> GetRelayIndexStack()
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
        private static void ValidateStrict(RelayIndex<TTarget> relayIndex)
        {
            if (!relayIndex.StrictSatisfied)
            {
                throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any redirects");
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object? CallRoot(CallInfo<TTarget> callInfo)
        {
            if (callInfo.Root == null)
            {
                throw new DiverterNullRootException("Root instance is null");
            }

            return callInfo.Invoke(callInfo.Root);
        }
    }
}