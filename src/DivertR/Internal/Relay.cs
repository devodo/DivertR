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

        public CallInfo<TTarget> CallInfo => GetCurrentStack().Peek().CallInfo;
        public Redirect<TTarget> Redirect => GetCurrentStack().Peek().Redirect;
        
        public TTarget Next => _next.Value;
        public TTarget Original => _original.Value;

        public Relay(IProxyFactory proxyFactory)
        {
            _next = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new NextProxyCall<TTarget>(this)));
            _original = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new OriginalProxyCall<TTarget>(this)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallBegin(RedirectPlan<TTarget> redirectPlan, CallInfo<TTarget> callInfo)
        {
            var redirectStep = BeginNewCall(redirectPlan, callInfo);

            if (redirectStep == null)
            {
                if (redirectPlan.IsStrictMode)
                {
                    throw new StrictNotSatisfiedException("Strict mode is enabled and the call did not match any redirects");
                }
                
                return CallOriginal(callInfo);
            }

            try
            {
                return redirectStep.Redirect.CallHandler.Call(callInfo);
            }
            finally
            {
                EndCall(callInfo);
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
        public object? CallOriginal(MethodInfo method, CallArguments callArguments)
        {
            var redirectStep = GetCurrentStack().Peek();
            ValidateStrict(redirectStep);
            var callInfo = new CallInfo<TTarget>(redirectStep.CallInfo.Proxy, redirectStep.CallInfo.Original, method, callArguments);
            
            return CallOriginal(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallOriginal(CallArguments callArguments)
        {
            var redirectStep = GetCurrentStack().Peek();
            ValidateStrict(redirectStep);
            var callInfo = new CallInfo<TTarget>(redirectStep.CallInfo.Proxy, redirectStep.CallInfo.Original, redirectStep.CallInfo.Method, callArguments);
            
            return CallOriginal(callInfo);
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
        public object? CallNext(MethodInfo method, CallArguments callArguments)
        {
            var redirectStack = GetCurrentStack();
            var redirectStep = redirectStack.Peek();
            var callInfo = new CallInfo<TTarget>(redirectStep.CallInfo.Proxy, redirectStep.CallInfo.Original, method, callArguments);
            var nextRedirectIndex = redirectStep.MoveNext(callInfo);

            if (nextRedirectIndex == null)
            {
                ValidateStrict(redirectStep);

                return CallOriginal(callInfo);
            }
            
            redirectStack = redirectStack.Push(nextRedirectIndex);
            _redirectStack.Value = redirectStack;

            try
            {
                return nextRedirectIndex.Redirect.CallHandler.Call(callInfo);
            }
            finally
            {
                _redirectStack.Value = redirectStack.Pop();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(CallArguments callArguments)
        {
            var redirectStack = GetCurrentStack();
            var redirectStep = redirectStack.Peek();
            var callInfo = new CallInfo<TTarget>(redirectStep.CallInfo.Proxy, redirectStep.CallInfo.Original, redirectStep.CallInfo.Method, callArguments);
            var nextRedirectIndex = redirectStep.MoveNext(callInfo);

            if (nextRedirectIndex == null)
            {
                ValidateStrict(redirectStep);

                return CallOriginal(callInfo);
            }
            
            redirectStack = redirectStack.Push(nextRedirectIndex);
            _redirectStack.Value = redirectStack;

            try
            {
                return nextRedirectIndex.Redirect.CallHandler.Call(callInfo);
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RelayStep<TTarget>? BeginNewCall(RedirectPlan<TTarget> redirectPlan, CallInfo<TTarget> callInfo)
        {
            var redirectStep = RelayStep<TTarget>.Create(redirectPlan, callInfo);

            if (redirectStep == null)
            {
                return null;
            }
            
            var redirectStack = _redirectStack.Value ?? ImmutableStack<RelayStep<TTarget>>.Empty;
            _redirectStack.Value = redirectStack.Push(redirectStep);
            
            return redirectStep;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EndCall(CallInfo<TTarget> callInfo)
        {
            _redirectStack.Value = _redirectStack.Value.Pop(out var redirectContext);

            if (!ReferenceEquals(callInfo, redirectContext.CallInfo))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current call");
            }
        }
    }
}