using DivertR.Core;

namespace DivertR.Internal
{
    internal class TrueCallConstraint : ICallConstraint
    {
        public static readonly TrueCallConstraint Instance = new TrueCallConstraint();
        
        public bool IsMatch(ICall call)
        {
            return true;
        }
    }
}
