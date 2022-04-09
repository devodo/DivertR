using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ViaProxyCall : IProxyCall
    {
        private readonly Relay _relay;
        private readonly RedirectPlan _redirectPlan;

        public ViaProxyCall(Relay relay, RedirectPlan redirectPlan)
        {
            _relay = relay;
            _redirectPlan = redirectPlan;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo callInfo)
        {
            return _relay.CallBegin(_redirectPlan, callInfo);
        }
    }
}