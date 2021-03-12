using DivertR.Core;

namespace DivertR.Internal
{
    internal class TrueCallCondition : ICallCondition
    {
        public static readonly TrueCallCondition Instance = new TrueCallCondition();
        
        public bool IsMatch(ICall call)
        {
            return true;
        }
    }
}
