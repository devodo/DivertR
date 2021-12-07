namespace DivertR
{
    public class SwitchCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        private readonly IRedirectSwitch _redirectSwitch;

        public SwitchCallConstraint(IRedirectSwitch redirectSwitch)
        {
            _redirectSwitch = redirectSwitch;
        }
        
        public bool IsMatch(CallInfo<TTarget> callInfo)
        {
            return _redirectSwitch.IsEnabled;
        }
    }
}