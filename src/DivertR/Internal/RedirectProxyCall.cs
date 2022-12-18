using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectProxyCall<TTarget> : IProxyCall<TTarget>
        where TTarget : class?
    {
        private readonly Relay<TTarget> _relay;
        private readonly IRedirectRepository _redirectRepository;

        public RedirectProxyCall(Relay<TTarget> relay, IRedirectRepository redirectRepository)
        {
            _relay = relay;
            _redirectRepository = redirectRepository;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call([DisallowNull] TTarget proxy, TTarget? root, MethodInfo method, CallArguments arguments)
        {
            var redirectPlan = _redirectRepository.RedirectPlan;

            if (redirectPlan == RedirectPlan.Empty)
            {
                return _relay.CallRoot(root, method, arguments);
            }
            
            var callInfo = new CallInfo<TTarget>(proxy, root, method, arguments);
            
            return _relay.CallBegin(redirectPlan, callInfo);
        }
    }
}