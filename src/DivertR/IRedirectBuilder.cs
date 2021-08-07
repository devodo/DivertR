using System;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        ICallConstraint<TTarget> CallConstraint { get; }
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirectBuilder<TTarget> AddRedirectDecorator(Func<IVia<TTarget>, IRedirect<TTarget>, IRedirect<TTarget>> decorator);
        IRedirect<TTarget> Build(TTarget target);
        IVia<TTarget> Redirect(TTarget target, int orderWeight = 0);
    }
}
