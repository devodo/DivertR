using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal class Relay<TTarget> : IRelay<TTarget> where TTarget : class
    {
        private readonly AsyncLocal<ImmutableStack<RelayStep<TTarget>>> _redirectStack = new AsyncLocal<ImmutableStack<RelayStep<TTarget>>>();
        
        private readonly Lazy<TTarget> _next;
        private readonly Lazy<TTarget> _original;
        
        public TTarget Next
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _next.Value;
        }

        public TTarget Original
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _original.Value;
        }
        
        public Relay(IProxyFactory proxyFactory)
        {
            _next = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new NextProxyCall<TTarget>(this)));
            _original = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new OriginalProxyCall<TTarget>(this)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallBegin(RedirectPlan<TTarget> redirectPlan, CallInfo<TTarget> callInfo)
        {
            var redirectStep = RelayStep<TTarget>.Create(redirectPlan, callInfo);

            if (redirectStep == null)
            {
                if (redirectPlan.IsStrictMode)
                {
                    throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any redirects");
                }
                
                return CallOriginal(callInfo);
            }
            
            var redirectStack = _redirectStack.Value ?? ImmutableStack<RelayStep<TTarget>>.Empty;
            redirectStack = redirectStack.Push(redirectStep);
            _redirectStack.Value = redirectStack;
            
            try
            {
                return redirectStep.Redirect.CallHandler.Call(callInfo);
            }
            finally
            {
                _redirectStack.Value = redirectStack.Pop();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallOriginal()
        {
            var redirectStep = GetCurrentStack().Peek();
            ValidateStrict(redirectStep);

            return CallOriginal(redirectStep.CallInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallOriginal(MethodInfo method, CallArguments args)
        {
            var redirectStep = GetCurrentStack().Peek();
            ValidateStrict(redirectStep);
            var callInfo = new CallInfo<TTarget>(redirectStep.CallInfo.Proxy, redirectStep.CallInfo.Original, method, args);
            
            return CallOriginal(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallOriginal(CallArguments args)
        {
            var redirectStep = GetCurrentStack().Peek();
            ValidateStrict(redirectStep);
            var callInfo = new CallInfo<TTarget>(redirectStep.CallInfo.Proxy, redirectStep.CallInfo.Original, redirectStep.CallInfo.Method, args);
            
            return CallOriginal(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRedirectCall<TTarget> GetCurrentCall()
        {
            return new RedirectCall<TTarget>(GetCurrentStack().Peek());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext()
        {
            var redirectStack = GetCurrentStack();
            var redirectStep = redirectStack.Peek();
            var nextStep = redirectStep.MoveNext(redirectStep.CallInfo);

            if (nextStep == null)
            {
                ValidateStrict(redirectStep);

                return CallOriginal(redirectStep.CallInfo);
            }
            
            redirectStack = redirectStack.Push(nextStep);
            _redirectStack.Value = redirectStack;

            try
            {
                return nextStep.Redirect.CallHandler.Call(redirectStep.CallInfo);
            }
            finally
            {
                _redirectStack.Value = redirectStack.Pop();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(MethodInfo method, CallArguments args)
        {
            var redirectStack = GetCurrentStack();
            var redirectStep = redirectStack.Peek();
            var callInfo = new CallInfo<TTarget>(redirectStep.CallInfo.Proxy, redirectStep.CallInfo.Original, method, args);
            var nextStep = redirectStep.MoveNext(callInfo);

            if (nextStep == null)
            {
                ValidateStrict(redirectStep);

                return CallOriginal(callInfo);
            }
            
            redirectStack = redirectStack.Push(nextStep);
            _redirectStack.Value = redirectStack;

            try
            {
                return nextStep.Redirect.CallHandler.Call(callInfo);
            }
            finally
            {
                _redirectStack.Value = redirectStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(CallArguments args)
        {
            var redirectStack = GetCurrentStack();
            var redirectStep = redirectStack.Peek();
            var callInfo = new CallInfo<TTarget>(redirectStep.CallInfo.Proxy, redirectStep.CallInfo.Original, redirectStep.CallInfo.Method, args);
            var nextStep = redirectStep.MoveNext(callInfo);

            if (nextStep == null)
            {
                ValidateStrict(redirectStep);

                return CallOriginal(callInfo);
            }
            
            redirectStack = redirectStack.Push(nextStep);
            _redirectStack.Value = redirectStack;

            try
            {
                return nextStep.Redirect.CallHandler.Call(callInfo);
            }
            finally
            {
                _redirectStack.Value = redirectStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableStack<RelayStep<TTarget>> GetCurrentStack()
        {
            var redirectStack = _redirectStack.Value;

            if (redirectStack == null || redirectStack.IsEmpty)
            {
                throw new DiverterException("Access to this member is only valid within the context of a redirect call");
            }

            return redirectStack;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateStrict(RelayStep<TTarget> relayStep)
        {
            if (!relayStep.StrictSatisfied)
            {
                throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any redirects");
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object? CallOriginal(CallInfo<TTarget> callInfo)
        {
            if (callInfo.Original == null)
            {
                throw new DiverterException("Original instance reference is null");
            }

            return callInfo.Invoke(callInfo.Original);
        }
    }

    internal class Relay<TTarget, TReturn> : IRelay<TTarget, TReturn> where TTarget : class
    {
        private readonly IRelay<TTarget> _relay;

        public Relay(IRelay<TTarget> relay)
        {
            _relay = relay;
        }

        public TTarget Next
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _relay.Next;
        }

        public TTarget Original
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _relay.Original;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRedirectCall<TTarget> GetCurrentCall()
        {
            return _relay.GetCurrentCall();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? IRelay<TTarget>.CallNext()
        {
            return _relay.CallNext();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TReturn CallNext(CallArguments args)
        {
            return (TReturn) _relay.CallNext(args)!;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TReturn CallOriginal()
        {
            return (TReturn) _relay.CallOriginal()!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TReturn CallOriginal(CallArguments args)
        {
            return (TReturn) _relay.CallOriginal(args)!;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TReturn CallNext()
        {
            return (TReturn) _relay.CallNext()!;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(MethodInfo method, CallArguments args)
        {
            return _relay.CallNext(method, args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? IRelay<TTarget>.CallNext(CallArguments args)
        {
            return _relay.CallNext(args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? IRelay<TTarget>.CallOriginal()
        {
            return _relay.CallOriginal();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallOriginal(MethodInfo method, CallArguments args)
        {
            return _relay.CallOriginal(method, args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? IRelay<TTarget>.CallOriginal(CallArguments args)
        {
            return _relay.CallOriginal(args);
        }
    }
}