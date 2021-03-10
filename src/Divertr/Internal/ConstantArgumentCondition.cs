using DivertR.Core;

namespace DivertR.Internal
{
    internal class ConstantArgumentCondition : IArgumentCondition
    {
        private readonly object? _constantValue;

        public ConstantArgumentCondition(object? constantValue)
        {
            _constantValue = constantValue;
        }
        
        public bool IsMatch(object? argument)
        {
            return Equals(_constantValue, argument);
        }
    }
}