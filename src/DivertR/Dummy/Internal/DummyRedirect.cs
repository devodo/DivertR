using System.Runtime.CompilerServices;

namespace DivertR.Dummy.Internal
{
    public class DummyRedirect : IRedirect
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
    }
}