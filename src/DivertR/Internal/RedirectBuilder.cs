using System;
using System.Linq.Expressions;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        protected CompositeCallConstraint<TTarget> CallConstraint { get; private set; } = CompositeCallConstraint<TTarget>.Empty;

        public RedirectBuilder(ICallConstraint<TTarget>? callConstraint = null)
        {
            if (callConstraint != null)
            {
                CallConstraint = CallConstraint.AddCallConstraint(callConstraint);
            }
        }

        public IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            CallConstraint = CallConstraint.AddCallConstraint(callConstraint);

            return this;
        }

        public IRedirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target);

            return Build(callHandler, optionsAction);
        }
        
        protected IRedirect<TTarget> Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(callHandler, optionsAction.Create());
        }

        private IRedirect<TTarget> Build(ICallHandler<TTarget> callHandler, IRedirectOptions<TTarget> redirectOptions)
        {
            return new Redirect<TTarget>(callHandler, CallConstraint, redirectOptions);
        }
    }

    internal class RedirectBuilder : IRedirectBuilder
    {
        protected CompositeCallConstraint CallConstraint { get; private set; } = CompositeCallConstraint.Empty;
        
        public RedirectBuilder(ICallConstraint? callConstraint = null)
        {
            if (callConstraint != null)
            {
                CallConstraint = CallConstraint.AddCallConstraint(callConstraint);
            }
        }
        
        public IRedirectBuilder AddConstraint(ICallConstraint callConstraint)
        {
            CallConstraint = CallConstraint.AddCallConstraint(callConstraint);

            return this;
        }

        public IRedirect Build(object target, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            throw new NotImplementedException();
        }
        
        protected IRedirect Build(ICallHandler callHandler, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            throw new NotImplementedException();
        }

        private IRedirect Build(ICallHandler callHandler, IRedirectOptions redirectOptions)
        {
            throw new NotImplementedException();
        }
    }
}
