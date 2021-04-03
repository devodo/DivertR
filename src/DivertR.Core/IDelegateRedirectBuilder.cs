using System;

namespace DivertR.Core
{
    public interface IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        ICallConstraint<TTarget> CallConstraint { get; }
        IRedirect<TTarget> Build(TTarget target);
        IRedirect<TTarget> Build(Delegate redirectDelegate);
        IVia<TTarget> To(TTarget target);
        IVia<TTarget> To(Delegate redirectDelegate);
    }
}