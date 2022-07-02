using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ViaProxyCall<TTarget> : IProxyCall<TTarget>
        where TTarget : class
    {
        private readonly Relay<TTarget> _relay;
        private readonly IRedirectPlan _redirectPlan;

        public ViaProxyCall(Relay<TTarget> relay, IRedirectPlan redirectPlan)
        {
            _relay = relay;
            _redirectPlan = redirectPlan;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(ICallInfo<TTarget> callInfo)
        {
            return _relay.CallBegin(_redirectPlan, callInfo);
        }
    }
}