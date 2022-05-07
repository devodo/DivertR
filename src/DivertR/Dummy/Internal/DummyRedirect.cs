using System.Runtime.CompilerServices;

namespace DivertR.Dummy.Internal
{
    public class DummyRedirect : IRedirectUntargeted
    {
        private readonly DummyCallHandler _callHandler = new DummyCallHandler();
        
        public int OrderWeight => 0;
        public bool DisableSatisfyStrict => false;

        public ICallConstraint CallConstraint
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TrueCallConstraint.Instance;
        }

        public ICallHandler CallHandler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _callHandler;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            throw new System.NotImplementedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            throw new System.NotImplementedException();
        }
    }
}