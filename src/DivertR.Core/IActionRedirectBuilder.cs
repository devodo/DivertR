using System;

namespace DivertR.Core
{
    public interface IActionRedirectBuilder<T> : IRedirectBuilder<T> where T : class
    {
        IVia<T> To(Action redirectDelegate);
        IVia<T> To<T1>(Action<T1> redirectDelegate);
    }
}