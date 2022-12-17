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
    
    public class TrueCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class?
    {
        public static readonly TrueCallConstraint<TTarget> Instance = new();
        
        private TrueCallConstraint() { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo<TTarget> callInfo)
        {
            return true;
        }
    }
}
