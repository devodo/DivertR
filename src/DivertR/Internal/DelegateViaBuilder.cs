using System;

namespace DivertR.Internal
{
    internal abstract class DelegateViaBuilder<TTarget> : ViaBuilder<TTarget>, IDelegateViaBuilder<TTarget> where TTarget : class
    {
        protected DelegateViaBuilder(IVia<TTarget> via, IDelegateRedirectBuilder<TTarget> redirectBuilder)
            : base(via, redirectBuilder)
        {
            RedirectBuilder = redirectBuilder;
        }

        public new IDelegateRedirectBuilder<TTarget> RedirectBuilder { get; }

        public IDelegateViaBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = RedirectBuilder.Build(redirectDelegate, optionsAction);
            Via.RedirectRepository.InsertRedirect(redirect);

            return this;
        }
    }
}
