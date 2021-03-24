using System.Reflection;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class MethodCallConstraint : ICallConstraint
    {
        private readonly MethodInfo _methodInfo;
        private readonly IMethodConstraint _methodConstraint;
        private readonly IArgumentConstraint[] _argumentConditions;

        public MethodCallConstraint(MethodInfo methodInfo, IMethodConstraint methodConstraint, IArgumentConstraint[] argumentConditions)
        {
            _methodInfo = methodInfo;
            _methodConstraint = methodConstraint;
            _argumentConditions = argumentConditions;
        }

        public bool IsMatch(CallInfo callInfo)
        {
            if (!_methodConstraint.IsMatch(callInfo.Method))
            {
                return false;
            }

            if (_argumentConditions.Length != callInfo.Arguments.Count)
            {
                return false;
            }

            for (var i = 0; i < _argumentConditions.Length; i++)
            {
                if (!_argumentConditions[i].IsMatch(callInfo.Arguments[i]))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
