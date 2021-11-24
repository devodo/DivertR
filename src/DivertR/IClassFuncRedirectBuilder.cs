using System;

namespace DivertR
{
    public interface IClassFuncRedirectBuilder<TTarget, TReturn> : IFuncRedirectBuilder<TTarget, TReturn>
        where TTarget : class
        where TReturn : class
    {
        IVia<TReturn> Divert(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
