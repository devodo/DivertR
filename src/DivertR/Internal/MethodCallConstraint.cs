using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class MethodCallConstraint : ICallConstraint
    {
        private readonly IMethodConstraint _methodConstraint;
        private readonly IArgumentConstraint[] _argumentConditions;

        public MethodCallConstraint(IMethodConstraint methodConstraint, IArgumentConstraint[] argumentConditions)
        {
            _methodConstraint = methodConstraint;
            _argumentConditions = argumentConditions;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
