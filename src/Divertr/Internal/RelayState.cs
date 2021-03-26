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
        
        public CallInfo<T> CallInfo => _callStack.Value.Peek().CallInfo;
        
        public IRedirect<T> Redirect => _callStack.Value.Peek().Redirect;
        
        public object? CallBegin(List<IRedirect<T>> redirects, CallInfo<T> callInfo)
        {
            var redirect = BeginNewCall(redirects, callInfo);

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
                EndCall(callInfo);
            }
        }

        public object? CallOriginal(CallInfo<T> callInfo)
        {
            if (callInfo.Original == null)
            {
                throw new DiverterException("Original instance reference is null");
            }
                
            return callInfo.Invoke(callInfo.Original);
        }

        public object? CallNext(CallInfo<T> callInfo)
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
        
        private IRedirect<T>? BeginNewCall(List<IRedirect<T>> redirects, CallInfo<T> callInfo)
        {
            var redirectState = RedirectState<T>.Create(redirects, callInfo);

            if (redirectState == null)
            {
                return null;
            }
            
            var callStack = _callStack.Value ?? ImmutableStack<RedirectState<T>>.Empty;
            _callStack.Value = callStack.Push(redirectState);
            
            return redirectState.Redirect;
        }

        private void EndCall(CallInfo<T> callInfo)
        {
            _callStack.Value = _callStack.Value.Pop(out var redirectState);

            if (!ReferenceEquals(callInfo, redirectState.CallInfo))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current call");
            }
        }

        private IRedirect<T>? BeginNextRedirect(CallInfo<T> callInfo)
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

        private void EndRedirect(CallInfo<T> invocation)
        {
            _callStack.Value = _callStack.Value.Pop(out var redirectState);
            
            if (!ReferenceEquals(invocation, redirectState.CallInfo))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current redirect");
            }
        }
    }
}