using System;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        ICallConstraint<TTarget> CallConstraint { get; }
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirectBuilder<TTarget> WithOrderWeight(int orderWeight);
        IRedirectBuilder<TTarget> AddRedirectDecorator(Func<IRedirect<TTarget>, IRedirect<TTarget>> decorator);
        IRedirect<TTarget> Build(TTarget target);
        IVia<TTarget> To(TTarget target);
    }
}
