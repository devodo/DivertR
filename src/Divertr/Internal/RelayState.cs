using System.Collections.Immutable;
using System.Collections.Generic;
using System.Threading;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class RelayState<T> : IRelayState<T> where T : class
    {
        private readonly AsyncLocal<ImmutableStack<RedirectState<T>>> _callStack = new AsyncLocal<ImmutableStack<RedirectState<T>>>();
        
        public T Proxy => _callStack.Value.Peek().Proxy;
        public T? Original => _callStack.Value.Peek().Original;

        public CallInfo CallInfo => _callStack.Value.Peek().CallInfo;
        
        public IRedirect<T> Redirect => _callStack.Value.Peek().Redirect;
        
        public object? CallBegin(T proxy, T? original, List<IRedirect<T>> redirects, CallInfo callInfo)
        {
            var redirect = BeginNewCall(proxy, original, redirects, callInfo);

            if (redirect == null)
            {
                return CallOriginal(original, callInfo);
            }

            try
            {
                return redirect.Call(callInfo);
            }
            finally
            {
                EndCall(callInfo);
            }
        }

        public object? CallOriginal(CallInfo callInfo)
        {
            var original = Original;

            return CallOriginal(original, callInfo);
        }

        public object? CallNext(CallInfo callInfo)
        {
            var redirect = BeginNextRedirect(callInfo);
            
            if (redirect == null)
            {
                return CallOriginal(callInfo);
            }

            try
            {
                return redirect.Call(callInfo);
            }
            finally
            {
                EndRedirect(callInfo);
            }
        }
        
        private IRedirect<T>? BeginNewCall(T proxy, T? original, List<IRedirect<T>> redirects, CallInfo callInfo)
        {
            var redirectState = RedirectState<T>.Create(proxy, original, redirects, callInfo);

            if (redirectState == null)
            {
                return null;
            }
            
            var callStack = _callStack.Value ?? ImmutableStack<RedirectState<T>>.Empty;
            _callStack.Value = callStack.Push(redirectState);
            
            return redirectState.Redirect;
        }

        private void EndCall(CallInfo callInfo)
        {
            _callStack.Value = _callStack.Value.Pop(out var redirectState);

            if (!ReferenceEquals(callInfo, redirectState.CallInfo))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current call");
            }
        }

        private IRedirect<T>? BeginNextRedirect(CallInfo callInfo)
        {
            var callStack = _callStack.Value;
            var redirectState = callStack.Peek().MoveNext(callInfo);

            if (redirectState == null)
            {
                return null;
            }
            
            _callStack.Value = callStack.Push(redirectState);

            return redirectState.Redirect;
        }

        private void EndRedirect(CallInfo invocation)
        {
            _callStack.Value = _callStack.Value.Pop(out var redirectState);
            
            if (!ReferenceEquals(invocation, redirectState.CallInfo))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current redirect");
            }
        }

        private static object? CallOriginal(T? original, CallInfo callInfo)
        {
            if (original == null)
            {
                throw new DiverterException("Proxy original instance reference is null");
            }
                
            return callInfo.Invoke(original);
        }
    }
}