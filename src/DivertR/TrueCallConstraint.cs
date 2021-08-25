namespace DivertR
{
    public class TrueCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        public static readonly TrueCallConstraint<TTarget> Instance = new TrueCallConstraint<TTarget>();
        
        public bool IsMatch(CallInfo<TTarget> callInfo)
        {
            return true;
        }
    }
}
