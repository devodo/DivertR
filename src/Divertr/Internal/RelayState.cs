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
        
        private IRedirect<T>? BeginCall(T? original, List<IRedirect<T>> redirects, ICall call)
        {
            var redirectState = RedirectState<T>.Create(original, redirects, call);

            if (redirectState == null)
            {
                return null;
            }
            
            var callStack = _callStack.Value ?? ImmutableStack<RedirectState<T>>.Empty;
            _callStack.Value = callStack.Push(redirectState);
            
            return redirectState.Current;
        }

        private void EndCall(ICall call)
        {
            _callStack.Value = _callStack.Value.Pop(out var redirectState);

            if (!ReferenceEquals(call, redirectState.Call))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state for the current call");
            }
        }

        public object? CallBegin(T? original, List<IRedirect<T>> redirects, ICall call)
        {
            var redirect = BeginCall(original, redirects, call);

            if (redirect == null)
            {
                if (original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }
                
                return call.Method.ToDelegate(typeof(T)).Invoke(original, call.Arguments);
            }

            try
            {
                return redirect.Invoke(call);
            }
            finally
            {
                EndCall(call);
            }
        }

        public object? InvokeNext(ICall call)
        {
            var redirect = BeginNextRedirect(call);
            
            if (redirect == null)
            {
                var original = Original;
                if (original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }
                
                return call.Method.ToDelegate(typeof(T)).Invoke(original, call.Arguments);
            }

            try
            {
                return redirect.Invoke(call);
            }
            finally
            {
                EndRedirect(call);
            }
        }

        private IRedirect<T>? BeginNextRedirect(ICall call)
        {
            var callStack = _callStack.Value;
            var redirectState = callStack.Peek().MoveNext(call);

            if (redirectState == null)
            {
                return null;
            }
            
            _callStack.Value = callStack.Push(redirectState);

            return redirectState.Current;
        }

        private void EndRedirect(ICall invocation)
        {
            _callStack.Value = _callStack.Value.Pop(out var redirectState);
            
            if (!ReferenceEquals(invocation, redirectState.Call))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state for the current call");
            }
        }

        public T? Original => _callStack.Value.Peek().Original;
        
        public IRedirect<T> Redirect => _callStack.Value.Peek().Current;
    }
}