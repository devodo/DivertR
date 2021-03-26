using System;

namespace DivertR.Core
{
    public interface IRedirectBuilder<T> where T : class
    {
        ICallConstraint<T> BuildCallConstraint();
        IRedirect<T> BuildRedirect(T target);
        IRedirect<T> BuildRedirect(Delegate redirectDelegate);
        IVia<T> To(T target);
        IVia<T> To(Delegate redirectDelegate);
    }
}