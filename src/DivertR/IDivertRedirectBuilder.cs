using System;

namespace DivertR
{
    public interface IDivertRedirectBuilder<TTarget, TReturn>
        where TTarget : class
        where TReturn : class
    {
        IVia<TReturn> Via(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}