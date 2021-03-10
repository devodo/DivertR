using DivertR.Core;

namespace DivertR.Internal
{
    internal class TrueArgumentCondition : IArgumentCondition
    {
        public static readonly TrueArgumentCondition Instance = new TrueArgumentCondition();
        
        public bool IsMatch(object? argument)
        {
            return true;
        }
    }
}