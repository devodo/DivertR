namespace DivertR.Internal
{
    internal class CallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        private readonly ICallConstraint _callConstraint;

        public CallConstraint(ICallConstraint callConstraint)
        {
            _callConstraint = callConstraint;
        }
        
        public bool IsMatch(ICallInfo<TTarget> callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}