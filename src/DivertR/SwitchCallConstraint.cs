namespace DivertR
{
    public class SwitchCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        private readonly IRedirectSwitch _redirectSwitch;

        public SwitchCallConstraint(IRedirectSwitch redirectSwitch)
        {
            _redirectSwitch = redirectSwitch;
        }
        
        public bool IsMatch(ICallInfo<TTarget> callInfo)
        {
            return _redirectSwitch.IsEnabled;
        }
    }
}