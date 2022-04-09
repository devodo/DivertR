using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal class Relay : IRelay
    {
        private readonly AsyncLocal<ImmutableStack<RelayIndex>> _relayIndexStack = new AsyncLocal<ImmutableStack<RelayIndex>>();
        
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
            return CreateRedirectCall(this, relayIndex.CallInfo, relayIndex.Redirect);
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
                return relayNext.Redirect.CallHandler.Call(CreateRedirectCall(this, relayIndex.CallInfo, relayNext.Redirect));
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
            var callInfo = relayIndex.CallInfo.Create(method, args);
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
                return relayNext.Redirect.CallHandler.Call(CreateRedirectCall(this, callInfo, relayNext.Redirect));
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
            var callInfo = relayIndex.CallInfo.Create(args);
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
                return relayNext.Redirect.CallHandler.Call(CreateRedirectCall(this, callInfo, relayNext.Redirect));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal object? CallBegin(RedirectPlan redirectPlan, CallInfo callInfo)
        {
            var relayIndex = RelayIndex.Create(redirectPlan, callInfo);

            if (relayIndex == null)
            {
                if (redirectPlan.IsStrictMode)
                {
                    throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any redirects");
                }
                
                return CallRoot(callInfo);
            }
            
            var relayIndexStack = _relayIndexStack.Value ?? ImmutableStack<RelayIndex>.Empty;
            relayIndexStack = relayIndexStack.Push(relayIndex);
            _relayIndexStack.Value = relayIndexStack;
            
            try
            {
                return relayIndex.Redirect.CallHandler.Call(CreateRedirectCall(this, callInfo, relayIndex.Redirect));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableStack<RelayIndex> GetRelayIndexStack()
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
        private static void ValidateStrict(RelayIndex relayIndex)
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

        protected virtual IRedirectCall CreateRedirectCall(IRelay relay, CallInfo callInfo, Redirect redirect)
        {
            return new RedirectCall(relay, callInfo, redirect);
        }
    }
    
    internal class Relay<TTarget> : Relay, IRelay<TTarget> where TTarget : class
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

        public Relay(IProxyFactory proxyFactory)
        {
            _next = new Lazy<TTarget>(() => proxyFactory.CreateProxy<TTarget>(new NextProxyCall(this)));
            _root = new Lazy<TTarget>(() => proxyFactory.CreateProxy<TTarget>(new RootProxyCall(this)));
        }
        
        protected override IRedirectCall CreateRedirectCall(IRelay relay, CallInfo callInfo, Redirect redirect)
        {
            return new RedirectCall<TTarget>((IRelay<TTarget>) relay, (CallInfo<TTarget>) callInfo, redirect);
        }
    }
}