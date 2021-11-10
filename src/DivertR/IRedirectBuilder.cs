using System;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        Redirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? options = null);
        IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? options = null);
    }
}
