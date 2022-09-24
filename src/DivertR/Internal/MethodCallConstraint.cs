using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class MethodCallConstraint : ICallConstraint
    {
        private readonly IMethodConstraint _methodConstraint;
        private readonly IArgumentConstraint[] _argumentConstraints;

        public MethodCallConstraint(IMethodConstraint methodConstraint, IArgumentConstraint[] argumentConstraints)
        {
            _methodConstraint = methodConstraint;
            _argumentConstraints = argumentConstraints;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            if (!_methodConstraint.IsMatch(callInfo.Method))
            {
                return false;
            }

            if (_argumentConstraints.Length != callInfo.Arguments.Count)
            {
                return false;
            }

            for (var i = 0; i < _argumentConstraints.Length; i++)
            {
                if (!_argumentConstraints[i].IsMatch(callInfo.Arguments[i]))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
