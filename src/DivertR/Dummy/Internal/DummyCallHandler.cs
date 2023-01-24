using System.Runtime.CompilerServices;

namespace DivertR.Dummy.Internal
{
    public class DummyCallHandler : ICallHandler
    {
        private readonly DummyValueFactory _dummyValueFactory = new();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall redirectCall)
        {
            return _dummyValueFactory.Create(redirectCall.CallInfo.Method.ReturnType);
        }
    }
}