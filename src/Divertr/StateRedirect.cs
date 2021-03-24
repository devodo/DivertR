﻿using System;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    public class StateRedirect<T> : IRedirect<T> where T : class
    {
        private readonly IRelay<T> _relay;
        private readonly ICallConstraint _callConstraint;
        public object? State { get; }

        public StateRedirect(object? state, IRelay<T> relay, ICallConstraint? callConstraint = null)
        {
            State = state;
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
            _callConstraint = callConstraint ?? TrueCallConstraint.Instance;
        }
        
        public object? Call(CallInfo callInfo)
        {
            return _relay.CallNext(callInfo);
        }

        public bool IsMatch(CallInfo callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}