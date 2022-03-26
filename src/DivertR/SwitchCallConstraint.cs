namespace DivertR
{
    public class SwitchCallConstraint : ICallConstraint
    {
        private readonly IRedirectSwitch _redirectSwitch;

        public SwitchCallConstraint(IRedirectSwitch redirectSwitch)
        {
            _redirectSwitch = redirectSwitch;
        }
        
        public bool IsMatch(CallInfo callInfo)
        {
            return _redirectSwitch.IsEnabled;
        }
    }
}