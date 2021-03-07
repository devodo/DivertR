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
        
        public IRedirect<T>? BeginCall(T? original, List<IRedirect<T>> redirects, ICall call)
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

        public void EndCall(ICall call)
        {
            _callStack.Value = _callStack.Value.Pop(out var redirectState);

            if (!ReferenceEquals(call, redirectState.Call))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect state for the current call");
            }
        }

        public IRedirect<T>? BeginNextRedirect(ICall call)
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

        public void EndRedirect(ICall invocation)
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