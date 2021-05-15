using System.Reflection;

namespace DivertR.Internal
{
    internal class ReferenceMethodConstraint : IMethodConstraint
    {
        private readonly MethodInfo _methodInfo;

        public ReferenceMethodConstraint(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }
        
        public bool IsMatch(MethodInfo methodInfo)
        {
            return ReferenceEquals(_methodInfo, methodInfo);
        }
    }
}