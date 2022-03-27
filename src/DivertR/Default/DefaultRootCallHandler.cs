using System.Runtime.CompilerServices;

namespace DivertR.Default
{
    internal class DefaultRootCallHandler : ICallHandler
    {
        private readonly IDefaultValueFactory _defaultValueFactory;

        public DefaultRootCallHandler(IDefaultValueFactory defaultValueFactory)
        {
            _defaultValueFactory = defaultValueFactory;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo callInfo)
        {
            return _defaultValueFactory.GetDefaultValue(callInfo.Method.ReturnType);
        }
    }
}