using System.Runtime.CompilerServices;

namespace DivertR
{
    public class TrueCallConstraint : ICallConstraint
    {
        public static readonly TrueCallConstraint Instance = new();
        
        private TrueCallConstraint() { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            return true;
        }
    }
}
