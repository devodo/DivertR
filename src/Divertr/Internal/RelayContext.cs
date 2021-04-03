using System.Collections.Immutable;
using System.Collections.Generic;
using System.Threading;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RelayContext<TTarget> where TTarget : class
    {
        private readonly AsyncLocal<ImmutableStack<RedirectContext<TTarget>>> _redirectStack = new AsyncLocal<ImmutableStack<RedirectContext<TTarget>>>();
        
        public CallInfo<TTarget> CallInfo => _redirectStack.Value.Peek().CallInfo;
        
        public IRedirect<TTarget> Redirect => _redirectStack.Value.Peek().Redirect;
        
        public object? CallBegin(IList<IRedirect<TTarget>> redirects, CallInfo<TTarget> callInfo)
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

        public object? CallOriginal(CallInfo<TTarget> callInfo)
        {
            if (callInfo.Original == null)
            {
                throw new DiverterException("Original instance reference is null");
            }
                
            return callInfo.Invoke(callInfo.Original);
        }

        public object? CallNext(CallInfo<TTarget> callInfo)
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
        
        private IRedirect<TTarget>? BeginNewCall(IList<IRedirect<TTarget>> redirects, CallInfo<TTarget> callInfo)
        {
            var redirectContext = RedirectContext<TTarget>.Create(redirects, callInfo);

            if (redirectContext == null)
            {
                return null;
            }
            
            var redirectStack = _redirectStack.Value ?? ImmutableStack<RedirectContext<TTarget>>.Empty;
            _redirectStack.Value = redirectStack.Push(redirectContext);
            
            return redirectContext.Redirect;
        }

        private void EndCall(CallInfo<TTarget> callInfo)
        {
            _redirectStack.Value = _redirectStack.Value.Pop(out var redirectContext);

            if (!ReferenceEquals(callInfo, redirectContext.CallInfo))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current call");
            }
        }

        private IRedirect<TTarget>? BeginNextRedirect(CallInfo<TTarget> callInfo)
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

        private void EndRedirect(CallInfo<TTarget> invocation)
        {
            _redirectStack.Value = _redirectStack.Value.Pop(out var redirectContext);
            
            if (!ReferenceEquals(invocation, redirectContext.CallInfo))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state ending the current redirect");
            }
        }
    }
}