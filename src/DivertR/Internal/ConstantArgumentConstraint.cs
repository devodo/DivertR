using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ConstantArgumentConstraint : IArgumentConstraint
    {
        private readonly object? _constantValue;

        public ConstantArgumentConstraint(object? constantValue)
        {
            _constantValue = constantValue;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(object? argument)
        {
            return Equals(_constantValue, argument);
        }
    }
}