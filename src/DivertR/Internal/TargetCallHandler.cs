using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class TargetCallHandler<TTarget> : CallHandler<TTarget> where TTarget : class
    {
        private readonly TTarget _target;

        public TargetCallHandler(TTarget target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Call(CallInfo<TTarget> callInfo)
        {
            return callInfo.Invoke(_target);
        }
    }
}