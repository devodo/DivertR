using System.Reflection;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class MethodCallConstraint<T> : ICallConstraint<T> where T : class
    {
        private readonly IMethodConstraint _methodConstraint;
        private readonly IArgumentConstraint[] _argumentConditions;

        public MethodCallConstraint(IMethodConstraint methodConstraint, IArgumentConstraint[] argumentConditions)
        {
            _methodConstraint = methodConstraint;
            _argumentConditions = argumentConditions;
        }

        public bool IsMatch(CallInfo<T> callInfo)
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
