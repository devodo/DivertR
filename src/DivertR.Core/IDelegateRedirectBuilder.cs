using System;

namespace DivertR.Core
{
    public interface IDelegateRedirectBuilder<T> where T : class
    {
        ICallConstraint<T> BuildCallConstraint();
        IRedirect<T> Build(T target);
        IRedirect<T> Build(Delegate redirectDelegate);
        IVia<T> To(T target);
        IVia<T> To(Delegate redirectDelegate);
    }
}