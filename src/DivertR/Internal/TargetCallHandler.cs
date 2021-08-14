using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class TargetCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly TTarget _target;

        public TargetCallHandler(TTarget target)
        {
            _target = target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            if (_target == null)
            {
                throw new DiverterException("The redirect instance reference is null");
            }
            
            return callInfo.Invoke(_target);
        }
    }
}