﻿using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class TargetCallHandler<TTarget> : CallHandler<TTarget> where TTarget : class?
    {
        private readonly TTarget _target;
        private readonly ICallInvoker _callInvoker;

        public TargetCallHandler(TTarget target, ICallInvoker callInvoker)
        {
            _target = target;
            _callInvoker = callInvoker;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Handle(IRedirectCall<TTarget> call)
        {
            return _callInvoker.Invoke(_target, call.CallInfo.Method, call.CallInfo.Arguments);
        }
    }
}