using System;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        ICallConstraint<TTarget> CallConstraint { get; }
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirectBuilder<TTarget> Chain(Func<IVia<TTarget>, IRedirect<TTarget>, IRedirect<TTarget>> chainLink);
        IRedirect<TTarget> Build(TTarget target);
        IVia<TTarget> Retarget(TTarget target, int orderWeight = 0);
    }
}
