using System;

namespace DivertR.Core
{
    public interface IRedirectBuilder<T> where T : class
    {
        ICallConstraint BuildCallConstraint();
        IVia<T> To(T target, object? state = null);
        IVia<T> To(Delegate redirectDelegate);
    }
}