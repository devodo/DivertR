using System.Runtime.CompilerServices;

namespace DivertR.Default.Internal
{
    internal class DefaultRootCallHandler : ICallHandler
    {
        private readonly IDefaultValueFactory _defaultValueFactory;

        public DefaultRootCallHandler(IDefaultValueFactory defaultValueFactory)
        {
            _defaultValueFactory = defaultValueFactory;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _defaultValueFactory.Create(call.CallInfo.Method.ReturnType);
        }
    }
}