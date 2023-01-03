using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal class Relay<TTarget> : IRelay<TTarget> where TTarget : class?
    {
        private readonly AsyncLocal<ImmutableStack<RelayIndex<TTarget>>> _relayIndexStack = new AsyncLocal<ImmutableStack<RelayIndex<TTarget>>>();
        
        private readonly ICallInvoker _callInvoker;
        private readonly Lazy<TTarget> _next;
        private readonly Lazy<TTarget> _root;
        
        public Relay(IProxyFactory proxyFactory, ICallInvoker callInvoker)
        {
            _callInvoker = callInvoker;
            _next = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new NextProxyCall<TTarget>(this)));
            _root = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new RootProxyCall<TTarget>(this)));
        }
        
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
            var callInfo = relayIndex.CallInfo.Clone(method, args);
            
            return CallRoot(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallRoot(CallArguments args)
        {
            var relayIndex = GetRelayIndexStack().Peek();
            ValidateStrict(relayIndex);
            var callInfo = relayIndex.CallInfo.Clone(args);
            
            return CallRoot(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRedirectCall<TTarget> GetCurrentCall()
        {
            var relayIndex = GetRelayIndexStack().Peek();
            
            return CreateRedirectCall(relayIndex.CallInfo);
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
                return relayNext.Via.Handle(CreateRedirectCall(relayIndex.CallInfo));
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
            var callInfo = relayIndex.CallInfo.Clone(method, args);
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
                return relayNext.Via.Handle(CreateRedirectCall(callInfo));
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
            var callInfo = relayIndex.CallInfo.Clone(args);
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
                return relayNext.Via.Handle(CreateRedirectCall(callInfo));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal object? CallBegin(IRedirectPlan redirectPlan, ICallInfo<TTarget> callInfo)
        {
            var relayIndex = RelayIndex<TTarget>.Create(redirectPlan, callInfo);

            if (relayIndex == null)
            {
                if (redirectPlan.IsStrictMode)
                {
                    throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any Vias");
                }
                
                return CallRoot(callInfo);
            }
            
            var relayIndexStack = _relayIndexStack.Value ?? ImmutableStack<RelayIndex<TTarget>>.Empty;
            relayIndexStack = relayIndexStack.Push(relayIndex);
            _relayIndexStack.Value = relayIndexStack;
            
            try
            {
                return relayIndex.Via.Handle(CreateRedirectCall(callInfo));
            }
            finally
            {
                _relayIndexStack.Value = relayIndexStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal object? CallRoot(TTarget? root, MethodInfo method, CallArguments arguments)
        {
            if (root == null)
            {
                throw new DiverterNullRootException("Root instance is null");
            }

            return _callInvoker.Invoke(root, method, arguments);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object? CallRoot(ICallInfo<TTarget> callInfo)
        {
            return CallRoot(callInfo.Root, callInfo.Method, callInfo.Arguments.InternalArgs);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableStack<RelayIndex<TTarget>> GetRelayIndexStack()
        {
            var relayIndexStack = _relayIndexStack.Value;

            if (relayIndexStack == null || relayIndexStack.IsEmpty)
            {
                // The AsyncLocal value has not been initialised in the current context, therefore the caller in not within a call on this Relay.
                throw new DiverterException("Access to this member is only valid within the context of a Redirect call");
            }

            return relayIndexStack;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateStrict(RelayIndex<TTarget> relayIndex)
        {
            if (!relayIndex.StrictSatisfied)
            {
                throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any Vias");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IRedirectCall<TTarget> CreateRedirectCall(ICallInfo<TTarget> callInfo)
        {
            return new RedirectCall<TTarget>(this, callInfo);
        }
    }
}