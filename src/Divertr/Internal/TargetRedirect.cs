﻿using DivertR.Core;

namespace DivertR.Internal
{
    internal class TargetRedirect<T> : IRedirect<T> where T : class
    {
        private readonly T _target;
        private readonly ICallConstraint _callConstraint;

        public TargetRedirect(T target, ICallConstraint? callConstraint = null)
        {
            _target = target;
            _callConstraint = callConstraint ?? TrueCallConstraint.Instance;
        }

        public object? Invoke(ICall call)
        {
            if (_target == null)
            {
                throw new DiverterException("The redirect instance reference is null");
            }
            
            return call.Method.ToDelegate(typeof(T)).Invoke(_target, call.Arguments);
        }

        public bool IsMatch(ICall call)
        {
            return _callConstraint.IsMatch(call);
        }
    }
}