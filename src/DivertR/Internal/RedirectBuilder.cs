using System;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        protected readonly IVia<TTarget> Via;

        private CompositeCallConstraint<TTarget> _callConstraint = CompositeCallConstraint<TTarget>.Empty;

        public RedirectBuilder(IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
        {
            Via = via ?? throw new ArgumentNullException(nameof(via));

            if (callConstraint != null)
            {
                _callConstraint = _callConstraint.AddCallConstraint(callConstraint);
            }
        }

        public IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            _callConstraint = _callConstraint.AddCallConstraint(callConstraint);

            return this;
        }

        public Redirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? options = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target);

            return Build(callHandler, options);
        }

        public IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? options = null)
        {
            var redirect = Build(target, options);
            
            return InsertRedirect(redirect);
        }

        protected Redirect<TTarget> Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var builder = new RedirectOptionsBuilder<TTarget>();
            optionsAction?.Invoke(builder);
            var options = builder.Build();
            callHandler = options.CallHandlerDecorator?.Invoke(Via, callHandler) ?? callHandler;

            return new Redirect<TTarget>(callHandler, _callConstraint, options.OrderWeight, options.DisableSatisfyStrict);
        }
        
        protected IVia<TTarget> InsertRedirect(Redirect<TTarget> redirect)
        {
            return Via.InsertRedirect(redirect);
        }
    }
}
