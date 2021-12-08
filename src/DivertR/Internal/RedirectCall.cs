using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectCall<TTarget> : IRedirectCall<TTarget> where TTarget : class
    {
        private readonly RelayStep<TTarget> _relayStep;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RedirectCall(RelayStep<TTarget> relayStep)
        {
            _relayStep = relayStep;
        }

        public CallInfo<TTarget> CallInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _relayStep.CallInfo;
        }

        public CallArguments Args
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CallInfo.Arguments;
        }

        public Redirect<TTarget> Redirect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _relayStep.Redirect;
        }
    }
}