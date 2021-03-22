using System;

namespace DivertR.Core
{
    public interface IRedirectBuilder<T> where T : class
    {
        ICallConstraint BuildCallConstraint();
        IVia<T> To(T target);
        IVia<T> To(Delegate redirectDelegate);
    }
}