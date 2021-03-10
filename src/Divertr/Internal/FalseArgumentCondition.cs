using DivertR.Core;

namespace DivertR.Internal
{
    internal class FalseArgumentCondition : IArgumentCondition
    {
        public static readonly FalseArgumentCondition Instance = new FalseArgumentCondition();
        
        public bool IsMatch(object? argument)
        {
            return false;
        }
    }
}