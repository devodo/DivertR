namespace DivertR.Internal
{
    internal class RedirectCall<TTarget> : IRedirectCall<TTarget> where TTarget : class
    {
        private readonly RelayStep<TTarget> _relayStep;

        public RedirectCall(RelayStep<TTarget> relayStep)
        {
            _relayStep = relayStep;
        }

        public CallInfo<TTarget> CallInfo => _relayStep.CallInfo;
        
        public CallArguments Args => CallInfo.Arguments;
        
        public Redirect<TTarget> Redirect => _relayStep.Redirect;
    }
}