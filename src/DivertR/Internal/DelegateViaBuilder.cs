using System;

namespace DivertR.Internal
{
    internal abstract class DelegateViaBuilder<TTarget> : ViaBuilder<TTarget>, IDelegateViaBuilder<TTarget> where TTarget : class
    {
        private readonly IDelegateRedirectBuilder<TTarget> _redirectBuilder;

        protected DelegateViaBuilder(IRedirectRepository redirectRepository, IDelegateRedirectBuilder<TTarget> redirectBuilder)
            : base(redirectRepository, redirectBuilder)
        {
            _redirectBuilder = redirectBuilder;
        }

        public IDelegateViaBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = _redirectBuilder.Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }
    }
}
