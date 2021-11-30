using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class TargetCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly TTarget _target;

        public TargetCallHandler(TTarget target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return callInfo.Invoke(_target);
        }
    }
}