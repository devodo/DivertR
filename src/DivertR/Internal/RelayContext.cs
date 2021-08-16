using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RelayContext<TTarget> where TTarget : class
    {
        private readonly AsyncLocal<ImmutableStack<RedirectIndex<TTarget>>> _redirectStack = new AsyncLocal<ImmutableStack<RedirectIndex<TTarget>>>();

        private RedirectIndex<TTarget> Current
        {
            get
            {
                var redirectStack = _redirectStack.Value;

                if (redirectStack == null || redirectStack.IsEmpty)
                {
                    throw new DiverterException("Access to this member is only valid within the context of a redirect call");
                }

                return redirectStack.Peek();
            }
        }
        
        public CallInfo<TTarget> CallInfo => Current.CallInfo;
        
        public Redirect<TTarget> Redirect => Current.Redirect;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallBegin(RedirectConfiguration<TTarget> redirectConfiguration, CallInfo<TTarget> callInfo)
        {
            var redirect = BeginNewCall(redirectConfiguration, callInfo);

            if (redirect == null)
            {
                if (redirectConfiguration.IsStrictMode)
                {
                    throw new DiverterException("Strict mode is enabled and the call did not match any redirects");
                }
                
                return CallOriginalUnchecked(callInfo);
            }

            try
            {
                return redirect.CallHandler.Call(callInfo);
            }
            finally
            {
                EndCall(callInfo);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallOriginal()
        {
            return CallOriginalChecked();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallOriginal(MethodInfo method, CallArguments callArguments)
        {
            return CallOriginalChecked((method, callArguments));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext()
        {
            return CallNextChecked();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(MethodInfo method, CallArguments callArguments)
        {
            return CallNextChecked((method, callArguments));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object? CallOriginalChecked((MethodInfo Method, CallArguments CallArguments)? call = null)
        {
            var redirectStack = _redirectStack.Value;

            if (redirectStack == null || redirectStack.IsEmpty)
            {
                throw new DiverterException("Access to this member is only valid within the context of a redirect call");
            }

            var redirectIndex = redirectStack.Peek();

            if (!redirectIndex.StrictSatisfied)
            {
                throw new DiverterException("Strict mode is enabled and the call did not match any redirects");
            }
            
            var callInfo = call == null 
                ? redirectIndex.CallInfo
                : new CallInfo<TTarget>(redirectIndex.CallInfo.Proxy, redirectIndex.CallInfo.Original, call.Value.Method, call.Value.CallArguments);
            
            return CallOriginalUnchecked(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object? CallNextChecked((MethodInfo Method, CallArguments CallArguments)? call = null)
        {
            var redirectStack = _redirectStack.Value;
            
            if (redirectStack == null || redirectStack.IsEmpty)
            {
                throw new DiverterException("Access to this member is only valid within the context of a redirect call");
            }
            
            var redirectIndex = redirectStack.Peek();
            var callInfo = call == null 
                ? redirectIndex.CallInfo
                : new CallInfo<TTarget>(redirectIndex.CallInfo.Proxy, redirectIndex.CallInfo.Original, call.Value.Method, call.Value.CallArguments);

            var nextRedirectIndex = redirectIndex.MoveNext(callInfo);

            if (nextRedirectIndex == null)
            {
                if (!redirectIndex.StrictSatisfied)
                {
                    throw new DiverterException("Strict mode is enabled and the call did not match any redirects");
                }
                
                return CallOriginalUnchecked(callInfo);
            }
            
            redirectStack = redirectStack.Push(nextRedirectIndex);
            _redirectStack.Value = redirectStack;

            try
            {
                return nextRedirectIndex.Redirect.CallHandler.Call(callInfo);
            }
            finally
            {
                _redirectStack.Value = redirectStack.Pop(out var poppedRedirectIndex);
            
                if (!ReferenceEquals(poppedRedirectIndex, nextRedirectIndex))
                {
                    throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current redirect");
                }
            }
        }

        private object? CallOriginalUnchecked(CallInfo<TTarget> callInfo)
        {
            if (callInfo.Original == null)
            {
                throw new DiverterException("Original instance reference is null");
            }

            return callInfo.Invoke(callInfo.Original);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Redirect<TTarget>? BeginNewCall(RedirectConfiguration<TTarget> redirectConfiguration, CallInfo<TTarget> callInfo)
        {
            var redirectContext = RedirectIndex<TTarget>.Create(redirectConfiguration, callInfo);

            if (redirectContext == null)
            {
                return null;
            }
            
            var redirectStack = _redirectStack.Value ?? ImmutableStack<RedirectIndex<TTarget>>.Empty;
            _redirectStack.Value = redirectStack.Push(redirectContext);
            
            return redirectContext.Redirect;
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