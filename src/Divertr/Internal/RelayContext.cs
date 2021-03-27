using System.Collections.Immutable;
using System.Collections.Generic;
using System.Threading;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class RelayContext<T> : IRelayContext<T> where T : class
    {
        private readonly AsyncLocal<ImmutableStack<RedirectContext<T>>> _redirectStack = new AsyncLocal<ImmutableStack<RedirectContext<T>>>();
        
        public CallInfo<T> CallInfo => _redirectStack.Value.Peek().CallInfo;
        
        public IRedirect<T> Redirect => _redirectStack.Value.Peek().Redirect;
        
        public object? CallBegin(IList<IRedirect<T>> redirects, CallInfo<T> callInfo)
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
        
        private IRedirect<T>? BeginNewCall(IList<IRedirect<T>> redirects, CallInfo<T> callInfo)
        {
            var redirectContext = RedirectContext<T>.Create(redirects, callInfo);

            if (redirectContext == null)
            {
                return null;
            }
            
            var redirectStack = _redirectStack.Value ?? ImmutableStack<RedirectContext<T>>.Empty;
            _redirectStack.Value = redirectStack.Push(redirectContext);
            
            return redirectContext.Redirect;
        }

        private void EndCall(CallInfo<T> callInfo)
        {
            _redirectStack.Value = _redirectStack.Value.Pop(out var redirectContext);

            if (!ReferenceEquals(callInfo, redirectContext.CallInfo))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current call");
            }
        }

        private IRedirect<T>? BeginNextRedirect(CallInfo<T> callInfo)
        {
            var redirectStack = _redirectStack.Value;
            var redirectContext = redirectStack.Peek().MoveNext(callInfo);

            if (redirectContext == null)
            {
                return null;
            }
            
            _redirectStack.Value = redirectStack.Push(redirectContext);

            return redirectContext.Redirect;
        }

        private void EndRedirect(CallInfo<T> invocation)
        {
            _redirectStack.Value = _redirectStack.Value.Pop(out var redirectContext);
            
            if (!ReferenceEquals(invocation, redirectContext.CallInfo))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current redirect");
            }
        }
    }
}