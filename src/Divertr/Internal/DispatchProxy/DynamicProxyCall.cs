using System.Reflection;

namespace DivertR.Internal.DispatchProxy
{
    public class DispatchProxyCall : ICall
    {
        private readonly MethodInfo _targetMethod;
        private readonly object[] _args;

        public DispatchProxyCall(MethodInfo targetMethod, object[] args)
        {
            _targetMethod = targetMethod;
            _args = args;
        }
    }
}