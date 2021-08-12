using System;
using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;

        private readonly List<Func<IVia<TTarget>, IRedirect<TTarget>, IRedirect<TTarget>>> _postBuildActions =
            new List<Func<IVia<TTarget>, IRedirect<TTarget>, IRedirect<TTarget>>>();
        
        private CompositeCallConstraint<TTarget> _callConstraint = CompositeCallConstraint<TTarget>.Empty;

        public RedirectBuilder(IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));

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

        public IRedirectBuilder<TTarget> AddPostBuildAction(Func<IVia<TTarget>, IRedirect<TTarget>, IRedirect<TTarget>> postBuild)
        {
            _postBuildActions.Add(postBuild);

            return this;
        }

        public ICallConstraint<TTarget> CallConstraint => _callConstraint;

        public IRedirect<TTarget> Build(TTarget target)
        {
            var redirect = new TargetRedirect<TTarget>(target, _callConstraint);

            return ApplyPostBuildActions(redirect);
        }
        
        public IVia<TTarget> Retarget(TTarget target, int orderWeight = 0)
        {
            var redirect = Build(target);
            
            return _via.InsertRedirect(redirect, orderWeight);
        }
        
        protected IVia<TTarget> InsertRedirect(IRedirect<TTarget> redirect, int orderWeight)
        {
            return _via.InsertRedirect(redirect, orderWeight);
        }
        
        protected IRedirect<TTarget> ApplyPostBuildActions(IRedirect<TTarget> redirect)
        {
            foreach (var postBuild in _postBuildActions)
            {
                redirect = postBuild.Invoke(_via, redirect);
            }

            return redirect;
        }
    }
}
