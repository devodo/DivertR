using System.Runtime.CompilerServices;

namespace DivertR
{
    public class RedirectUntargeted : IRedirectUntargeted
    {
        public RedirectUntargeted(ICallHandler callHandler, ICallConstraint? callConstraint = null, int? orderWeight = null, bool? disableSatisfyStrict = null)
        {
            CallHandler = callHandler;
            CallConstraint = callConstraint ?? TrueCallConstraint.Instance;
            OrderWeight = orderWeight ?? 0;
            DisableSatisfyStrict = disableSatisfyStrict ?? false;
        }

        public int OrderWeight { get; }

        public bool DisableSatisfyStrict
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public ICallConstraint CallConstraint
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public ICallHandler CallHandler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            return CallConstraint.IsMatch(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return CallHandler.Handle(call);
        }
    }
}