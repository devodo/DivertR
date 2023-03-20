using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class Via : IVia
    {
        private readonly ICallHandler _callHandler;
        private readonly ICallConstraint _callConstraint;

        public Via(ICallHandler callHandler, ICallConstraint? callConstraint = null)
        {
            _callHandler = callHandler ?? throw new ArgumentNullException(nameof(callHandler));
            _callConstraint = callConstraint ?? TrueCallConstraint.Instance;
        }
        
        public Via(ICallHandler callHandler)
            : this(callHandler, TrueCallConstraint.Instance)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _callHandler.Handle(call);
        }
    }
}