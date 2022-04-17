using System.Runtime.CompilerServices;

namespace DivertR
{
    public class TrueCallConstraint : ICallConstraint
    {
        public static readonly TrueCallConstraint Instance = new TrueCallConstraint();
        
        private TrueCallConstraint() { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo callInfo)
        {
            return true;
        }
    }
    
    public class TrueCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        public static readonly TrueCallConstraint<TTarget> Instance = new TrueCallConstraint<TTarget>();
        
        private TrueCallConstraint() { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo<TTarget> callInfo)
        {
            return true;
        }
    }
}
