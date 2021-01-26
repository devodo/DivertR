﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Divertr
{
    internal class CallContext<T> : ICallContext<T> where T : class
    {
        private readonly AsyncLocal<List<RedirectionContext<T>>> _callStack = new AsyncLocal<List<RedirectionContext<T>>>();

        public CallContext()
        {
            Replaced = ProxyFactory.Instance.CreateRedirectProxy(this);
        }

        public T Replaced { get; }

        public T Original
        {
            get
            {
                var redirectionContext = Peek();

                if (redirectionContext == null)
                {
                    throw new InvalidOperationException("This instance may only be accessed in the context of a Diverter Proxy call");
                }

                return redirectionContext.Origin;
            }
        }

        public void Push(RedirectionContext<T> redirectionContext)
        {
            var invocationStack = _callStack.Value?.ToList() ?? new List<RedirectionContext<T>>();
            invocationStack.Add(redirectionContext);
            _callStack.Value = invocationStack;
        }

        public RedirectionContext<T> Pop()
        {
            var invocationStack = _callStack.Value;

            if (invocationStack == null || invocationStack.Count == 0)
            {
                return null;
            }

            var invocationState = invocationStack[^1];
            invocationStack.RemoveAt(invocationStack.Count - 1);

            return invocationState;
        }

        public RedirectionContext<T> Peek()
        {
            var invocationStack = _callStack.Value;

            return invocationStack?[^1];
        }
    }
}