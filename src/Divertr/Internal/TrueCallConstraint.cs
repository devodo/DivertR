using DivertR.Core;

namespace DivertR.Internal
{
    internal class TrueCallConstraint : ICallConstraint
    {
        public static readonly TrueCallConstraint Instance = new TrueCallConstraint();
        
        public bool IsMatch(CallInfo callInfo)
        {
            return true;
        }
    }
}
