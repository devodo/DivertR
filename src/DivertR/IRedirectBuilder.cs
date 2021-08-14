using System;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirectBuilder<TTarget> ChainCallHandler(Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>> chainLink);
        IRedirectBuilder<TTarget> WithOrderWeight(int orderWeight);
        IRedirectBuilder<TTarget> WithExcludeStrict(bool excludeStrict = true);
        Redirect<TTarget> Build(TTarget target);
        IVia<TTarget> Retarget(TTarget target);
    }
}
